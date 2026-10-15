#!/usr/bin/env bash
set -euo pipefail
cd "$(dirname "$0")/.."
DB=airy_panel_dev
PORT=8202
PG="docker exec airy-postgres-db psql -U airy_user -d postgres -tAc"
if [ -z "$($PG "select 1 from pg_database where datname='$DB'")" ]; then
  DUMP=$(ls -t ~/backups/hive-prod-*/airy_db.dump | head -1)
  echo "creating $DB from $DUMP"
  $PG "create database $DB" >/dev/null
  docker exec -i airy-postgres-db pg_restore -U airy_user -d "$DB" --no-owner < "$DUMP"
fi
CS=$(python3 - "$DB" <<'PY'
import json, sys
d = json.load(open('AiryBotCode.Api/appsettings.json'))
parts = dict(p.split('=', 1) for p in d['ConnectionStrings']['DefaultConnection'].split(';') if p)
parts.update(Host='127.0.0.1', Port='5432', Database=sys.argv[1])
print(';'.join(f'{k}={v}' for k, v in parts.items()))
PY
)
export ConnectionStrings__DefaultConnection="$CS" Database__Host=127.0.0.1 Database__Port=5432 Database__Name=$DB
export ASPNETCORE_ENVIRONMENT=Production ASPNETCORE_URLS=http://127.0.0.1:$PORT OTEL_SDK_DISABLED=true
exec dotnet run --no-launch-profile --project AiryBotCode.Api
