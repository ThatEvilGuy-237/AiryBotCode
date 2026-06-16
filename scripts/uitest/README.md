# Airy panel browser tests

Headless Playwright tests that drive the **real** control panel in Chromium —
for UI flows that can't be checked by build/serve alone (e.g. the image→theme
upload → bot-switch → re-theme persistence).

Reuses the Hive's `hive-uitest` image (Playwright baked in). Auth is a JWT minted
with the API's `Jwt:Secret` (== The-Hive's `AIRY_JWT_SECRET`), injected as the
`jwt` cookie the panel reads.

## Run (against the LOCAL dev panel on :10000)

```bash
cd ~/AiryBotCode/scripts/uitest
SECRET=$(grep -oP '^\s*AIRY_JWT_SECRET\s*=\s*\K.+' ~/The-Hive/.env | tr -d '"')
JWT=$(AIRY_JWT_SECRET="$SECRET" node mint-jwt.cjs)
mkdir -p /tmp/airy-uitest
docker run --rm --network host -e JWT="$JWT" -e BASE=http://127.0.0.1:10000 \
  -v "$PWD:/work" -v /tmp/airy-uitest:/out hive-uitest node /work/theme.cjs
# screenshots land in /tmp/airy-uitest/airy-*.png
```

`--network host` lets the container reach the host-bound dev panel on
`127.0.0.1:10000`. Build the image once if missing: `docker build -t hive-uitest ~/The-Hive/scripts/uitest`.

## Note: dev needs a bot row

The per-bot theme Save is disabled when no bot is selected, and the dev stack has
no bot to seed `BotSettings`. Seed one into the isolated dev DB to exercise the
full flow:

```bash
docker exec airybotcode-dev-db-dev-1 psql -U airy_user -d airy_db_dev -c \
  "INSERT INTO \"BotSettings\" (\"BotId\",\"BotName\",\"Enabled\",\"EvilId\",\"LogChannelId\",\"EvilLogChannelId\") VALUES (1,'Dev Test Bot',true,0,0,0) ON CONFLICT DO NOTHING;"
```
