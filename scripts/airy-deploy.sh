#!/usr/bin/env bash
# AiryBotCode deploy helper.
#
#   ./scripts/airy-deploy.sh dev      # the dev stack from this checkout (LOCAL 127.0.0.1:10000, own Postgres)
#   ./scripts/airy-deploy.sh dev-bot  # dev stack + the OPTIONAL dev bot (needs AIRY_DEVBOT_DEV_TOKEN)
#   ./scripts/airy-deploy.sh prod     # main → the api service and the panel files, nothing else
#
# prod never touches airydevbot (the bot is off by the owner's decision), never airy-postgres-db, and never
# restarts Caddy (it also serves The Hive): it builds the api and caddy images, recreates the api service alone
# (--no-deps), then copies the new panel files into the running Caddy's /srv. The rebuilt Caddy image holds the
# same files for its next restart. The previous images stay as :prev.
#
# AiryBotCode is a PUBLIC repo: appsettings.json, .env and every credential stay out of git
# (credentials live in /mnt/hdd/projects/AiryBotCode-data, mode 600).
set -euo pipefail
cd "$(dirname "$0")/.."

caddy_addresses() {
  docker inspect "$1" --format '{{range .NetworkSettings.Networks}}{{.IPAddress}} {{end}}'
}

wait_ready() {
  local url=$1 host=$2
  for _ in $(seq 1 60); do
    if [ "$(curl -s -o /dev/null -w '%{http_code}' -H "Host: $host" "$url")" = "200" ]; then return 0; fi
    sleep 2
  done
  echo "✗ $url did not answer 200" >&2
  return 1
}

deploy_dev() {
  local project=(-f docker-compose.dev.yml -p airybotcode-dev)
  docker compose "${project[@]}" up -d --build db-dev caddy-dev
  read -r -a dev_caddy <<< "$(caddy_addresses airybotcode-dev-caddy-dev-1)"
  AIRY_DEV_CADDY_ADDRESS="${dev_caddy[0]}" docker compose "${project[@]}" up -d --build --no-deps api-dev
  wait_ready http://127.0.0.1:10000/api/ready localhost:10000
  echo "✓ dev running (local only) → http://127.0.0.1:10000"
}

deploy_dev_bot() {
  if ! grep -q '^AIRY_DEVBOT_DEV_TOKEN=.\+' .env 2>/dev/null; then
    echo "⚠ AIRY_DEVBOT_DEV_TOKEN is not set in .env — set a SEPARATE dev bot token first."
  fi
  docker compose -f docker-compose.dev.yml -p airybotcode-dev --profile bot up -d --build
  echo "✓ dev + dev bot running (local only) → http://127.0.0.1:10000"
}

deploy_prod() {
  [ "$(git rev-parse --abbrev-ref HEAD)" = "main" ] || { echo "✗ prod deploys main only" >&2; exit 1; }
  [ -z "$(git status --porcelain --untracked-files=no)" ] || { echo "✗ the tree has uncommitted changes" >&2; exit 1; }
  local commit
  commit="$(git rev-parse --short HEAD)"
  for image in airybotcode-api airybotcode-caddy; do
    docker image inspect "$image:latest" >/dev/null 2>&1 && docker tag "$image:latest" "$image:prev"
  done
  AIRY_COMMIT="$commit" docker compose -f docker-compose.yml -p airybotcode build api caddy
  read -r -a caddy <<< "$(caddy_addresses airybotcode-caddy-1)"
  AIRY_COMMIT="$commit" AIRY_CADDY_ADDRESS_0="${caddy[0]:-}" AIRY_CADDY_ADDRESS_1="${caddy[1]:-}" \
    docker compose -f docker-compose.yml -p airybotcode up -d --no-deps --no-build api
  wait_ready http://127.0.0.1:8088/api/ready airy.thatevilguy.com
  local panel
  panel="$(mktemp -d)"
  local holder
  holder="$(docker create airybotcode-caddy:latest)"
  docker cp "$holder:/srv/." "$panel/"
  docker rm "$holder" >/dev/null
  docker cp "$panel/." airybotcode-caddy-1:/srv/
  rm -rf "$panel"
  echo "✓ prod deployed $commit → https://airy.thatevilguy.com (api recreated, panel files copied, Caddy not restarted)"
}

case "${1:-}" in
  dev)      deploy_dev ;;
  dev-bot)  deploy_dev_bot ;;
  prod)     deploy_prod ;;
  *)
    echo "usage: $0 {dev|dev-bot|prod}"; exit 1 ;;
esac
