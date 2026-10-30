#!/usr/bin/env python3
"""Give Airy's API its own Postgres role (airybotcode_app) instead of the superuser. Run once; safe to run again.

The role logs in only to airy_db (pg_hba: allowed there, refused everywhere else) and has data rights on airy_db's public
schema: read and write every table and sequence, including the ones the bot creates later (default privileges for
airy_user). No DDL, no other schema, no superuser. The password reaches Postgres as a SCRAM verifier and is written only
into /mnt/hdd/projects/AiryBotCode-data/airy-db.env (600) as the API's connection string. Connects as airy_user through
the container's local socket.
"""
import base64
import hashlib
import hmac
import os
import secrets
import subprocess
from pathlib import Path

INSTANCE = os.environ.get("AIRY_PG_INSTANCE", "airy-postgres-db")
ROLE = "airybotcode_app"
DATABASE = "airy_db"
STORE = Path(os.environ.get("AIRY_DB_STORE", "/mnt/hdd/projects/AiryBotCode-data/airy-db.env"))
KEY = "ConnectionStrings__DefaultConnection="
HBA_MARK = "# airybotcode_app: airy_db only (AiryBotCode scripts/airy-db-role.py)"


def psql(statement, database=DATABASE):
    done = subprocess.run(["docker", "exec", "-i", INSTANCE, "psql", "-X", "-U", "airy_user", "-d", database, "-v", "ON_ERROR_STOP=1", "-A", "-t", "-q"],
                          input=statement, capture_output=True, text=True)
    if done.returncode != 0:
        raise SystemExit(f"psql failed: {done.stderr.strip()}")
    return done.stdout.strip()


def scram(password):
    salt = secrets.token_bytes(16)
    salted = hashlib.pbkdf2_hmac("sha256", password.encode(), salt, 4096)
    client = hmac.new(salted, b"Client Key", hashlib.sha256).digest()
    server = hmac.new(salted, b"Server Key", hashlib.sha256).digest()
    b64 = lambda raw: base64.b64encode(raw).decode()
    return f"SCRAM-SHA-256$4096:{b64(salt)}${b64(hashlib.sha256(client).digest())}:{b64(server)}"


def stored_password():
    if not STORE.exists():
        return None
    for line in STORE.read_text().splitlines():
        if line.startswith(KEY):
            return dict(p.split("=", 1) for p in line[len(KEY):].split(";") if "=" in p).get("Password")
    return None


def write_store(password):
    STORE.parent.mkdir(parents=True, exist_ok=True)
    STORE.parent.chmod(0o700)
    old = os.umask(0o077)
    try:
        STORE.write_text(f"{KEY}Host=db;Port=5432;Database={DATABASE};Username={ROLE};Password={password}\n")
    finally:
        os.umask(old)
    STORE.chmod(0o600)


def ensure_role():
    exists = psql(f"SELECT count(*) FROM pg_roles WHERE rolname = '{ROLE}'") == "1"
    password = stored_password()
    if exists and password:
        print(f"  {ROLE}: exists, secret in {STORE}")
        return
    password = secrets.token_hex(24)
    flags = "LOGIN NOSUPERUSER NOCREATEDB NOCREATEROLE NOBYPASSRLS NOREPLICATION NOINHERIT"
    psql(f"{'ALTER' if exists else 'CREATE'} ROLE {ROLE} {flags} PASSWORD '{scram(password)}'")
    write_store(password)
    print(f"  {ROLE}: {'password set' if exists else 'created'}, secret written to {STORE}")


def ensure_grants():
    psql(f"""
        GRANT CONNECT ON DATABASE {DATABASE} TO {ROLE};
        GRANT USAGE ON SCHEMA public TO {ROLE};
        GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA public TO {ROLE};
        GRANT USAGE, SELECT, UPDATE ON ALL SEQUENCES IN SCHEMA public TO {ROLE};
        ALTER DEFAULT PRIVILEGES FOR ROLE airy_user IN SCHEMA public GRANT SELECT, INSERT, UPDATE, DELETE ON TABLES TO {ROLE};
        ALTER DEFAULT PRIVILEGES FOR ROLE airy_user IN SCHEMA public GRANT USAGE, SELECT, UPDATE ON SEQUENCES TO {ROLE};
    """)
    tables = psql(f"SELECT count(*) FROM information_schema.role_table_grants WHERE grantee = '{ROLE}' AND table_schema = 'public' AND privilege_type = 'SELECT'")
    print(f"  grants: data rights on {DATABASE}.public ({tables} tables), default privileges for airy_user's new tables")


def ensure_hba():
    as_postgres = ["docker", "exec", "-i", "-u", "postgres", INSTANCE, "sh", "-c"]
    current = subprocess.run([*as_postgres, 'cat "$PGDATA/pg_hba.conf"'], capture_output=True, text=True, check=True).stdout
    block = [HBA_MARK, f"host {DATABASE} {ROLE} all scram-sha-256", f"host all {ROLE} all reject", f"{HBA_MARK} end"]
    lines = current.splitlines()
    if HBA_MARK in lines:
        start, end = lines.index(HBA_MARK), lines.index(f"{HBA_MARK} end")
        lines[start:end + 1] = block
    else:
        first = min(i for i, line in enumerate(lines) if line.split()[:4] == ["host", "all", "all", "all"])
        lines[first:first] = block
    new = "\n".join(lines) + "\n"
    if new != current:
        subprocess.run([*as_postgres, 'cp -p "$PGDATA/pg_hba.conf" "$PGDATA/pg_hba.conf.before-airybotcode-app"'], check=True)
        subprocess.run([*as_postgres, 'umask 077 && cat > "$PGDATA/pg_hba.conf.next" && mv "$PGDATA/pg_hba.conf.next" "$PGDATA/pg_hba.conf"'], input=new, text=True, check=True)
    owner = subprocess.run([*as_postgres, 'stat -c %U:%a "$PGDATA/pg_hba.conf"'], capture_output=True, text=True, check=True).stdout.strip()
    errors = psql("SELECT count(*) FROM pg_hba_file_rules WHERE error IS NOT NULL")
    if errors != "0" or owner != "postgres:600":
        subprocess.run([*as_postgres, 'cp -p "$PGDATA/pg_hba.conf.before-airybotcode-app" "$PGDATA/pg_hba.conf"'], check=True)
        raise SystemExit(f"pg_hba: the new file does not parse or has the wrong owner ({owner}); the old file is back, nothing reloaded")
    psql("SELECT pg_reload_conf();")
    print(f"  pg_hba: rules {'added' if new != current else 'in place'}, file {owner}, reloaded (the file before them is pg_hba.conf.before-airybotcode-app)")


def main():
    if psql("SELECT current_setting('log_statement')") != "none":
        raise SystemExit("log_statement is not 'none' on the instance: refusing, the SCRAM verifier must stay out of the log")
    ensure_role()
    ensure_grants()
    ensure_hba()
    print("done")


if __name__ == "__main__":
    main()
