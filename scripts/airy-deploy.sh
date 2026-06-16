#!/usr/bin/env bash
# AiryBotCode — deploy + promote helper. Mirrors The Hive's scripts/hive-deploy.sh.
#
#   ./scripts/airy-deploy.sh dev      # build+run the feature branch → airybotcode-dev (LOCAL :10000)
#   ./scripts/airy-deploy.sh prod     # build+run `main` → airybotcode stack (/)  [production, online]
#   ./scripts/airy-deploy.sh promote  # merge feature branch → main (ff), push, then deploy prod
#
# Workflow: commit on the feature branch → `dev` runs it on the LOCAL-ONLY dev
# panel (127.0.0.1:10000, not public) for you to test → when a feature is good,
# `promote` merges it to main and deploys prod FROM main. Production (online)
# always tracks `main`; dev is local-only.
#
# Two parallel environments, fully isolated:
#   dev   = feature branch, project airybotcode-dev, own Postgres, panel+API only, LOCAL :10000
#   prod  = branch `main`,  project airybotcode,     airy-postgres-db, panel+API+bot, online /
#
# AiryBotCode is a PUBLIC repo: appsettings.json (secrets) is gitignored and must
# stay that way. This script never commits — it only merges already-committed work.
set -euo pipefail
cd "$(dirname "$0")/.."

# The feature branch dev work lands on. Override with FEATURE_BRANCH=... if needed.
FEATURE_BRANCH="${FEATURE_BRANCH:-channel-webhooks}"

deploy_dev() {
  docker compose -f docker-compose.dev.yml -p airybotcode-dev up -d --build
  echo "✓ dev running (local only) → http://127.0.0.1:10000"
}

deploy_prod() {
  git checkout main
  git pull --ff-only origin main || true
  docker compose -f docker-compose.yml -p airybotcode up -d --build
  echo "✓ prod deployed → https://thatevilserver.tail2a87af.ts.net"
}

case "${1:-}" in
  dev)  deploy_dev ;;
  prod) deploy_prod ;;
  promote)
    echo "Promoting $FEATURE_BRANCH → main…"
    git checkout main && git pull --ff-only origin main || true
    git merge --ff-only "$FEATURE_BRANCH"
    git push origin main
    deploy_prod
    ;;
  *)
    echo "usage: $0 {dev|prod|promote}"; exit 1 ;;
esac
