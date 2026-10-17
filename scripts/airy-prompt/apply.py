"""Put the prompt sections kept in git (docs/conversation/prompt) into Airy's live agent prompt on the prod Hive.

Dry run by default: prints the diff. --apply writes it through the Spine API and reads it back.
Persona, memory and continuity stay in the Hive; only sections that have a file here are replaced.
"""
import argparse
import difflib
import json
import os
import re
import subprocess
import sys
import urllib.request
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
SECTIONS = ROOT / "docs/conversation/prompt"
HIVE = Path.home() / "workspace-claude/The-Hive"
SPINE = "http://127.0.0.1:7000"
SPINE_CONTAINER = "hive-core-spine-1"
TENANT_DB = "hive_cookie_cream"
TENANT = "60376c50-60b8-458d-aee9-e0a8d4414624"
AGENT = "2d7b492e-27f2-49e0-8261-c9330aa3df60"


def sql(db, query):
    out = subprocess.run(["docker", "exec", "airy-postgres-db", "psql", "-U", "airy_user", "-d", db, "-tAc", query],
                         capture_output=True, text=True, check=True)
    return out.stdout.strip()


def token():
    env = subprocess.run(["docker", "inspect", SPINE_CONTAINER, "-f", "{{range .Config.Env}}{{println .}}{{end}}"],
                         capture_output=True, text=True, check=True).stdout
    secret = next(line.split("=", 1)[1] for line in env.splitlines() if line.startswith("Jwt__Secret="))
    owner = sql(TENANT_DB, "select m.account_id from spine.workspace_member m join spine.role r on r.id = m.role_id "
                           "where r.name ilike 'owner' limit 1")
    script = f"import('{HIVE}/scripts/lib-spine-token.mjs').then(m => process.stdout.write(m.mintSpineToken(process.env.S, process.env.A)))"
    return subprocess.run(["node", "-e", script], capture_output=True, text=True, check=True,
                          env={**os.environ, "S": secret, "A": owner}).stdout


def call(method, path, bearer, body=None):
    request = urllib.request.Request(f"{SPINE}{path}", method=method, data=None if body is None else json.dumps(body).encode(),
                                     headers={"Authorization": f"Bearer {bearer}", "X-Hive-Workspace": TENANT,
                                              "content-type": "application/json"})
    with urllib.request.urlopen(request, timeout=30) as response:
        return json.load(response)


def merge(live, sections):
    parts = re.split(r"(?m)^(?=## )", live.rstrip("\n"))
    for path in sorted(sections.glob("*.md")):
        text = path.read_text().rstrip("\n")
        heading = text.splitlines()[0]
        found = [i for i, part in enumerate(parts) if part.splitlines()[0] == heading]
        if len(found) != 1:
            sys.exit(f"{path.name}: heading {heading!r} found {len(found)} times in the live prompt")
        parts[found[0]] = text + "\n\n"
    return "".join(parts).rstrip("\n") + "\n"


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--apply", action="store_true", help="write the merged prompt to the Hive")
    args = parser.parse_args()
    bearer = token()
    agent = call("GET", f"/agents/{AGENT}", bearer)
    live = agent["systemPrompt"]
    merged = merge(live, SECTIONS)
    diff = list(difflib.unified_diff(live.rstrip("\n").splitlines(), merged.rstrip("\n").splitlines(), "live", "git", lineterm=""))
    if not diff:
        print("The live prompt already matches git.")
        return
    print("\n".join(diff))
    if not args.apply:
        print("\nDry run. Run again with --apply to write it.")
        return
    body = {k: v for k, v in agent.items() if k not in ("id", "createdAt", "updatedAt")}
    body["systemPrompt"] = merged
    call("PUT", f"/agents/{AGENT}", bearer, body)
    after = call("GET", f"/agents/{AGENT}", bearer)
    if after["systemPrompt"].rstrip("\n") != merged.rstrip("\n"):
        sys.exit("The Hive did not keep the new prompt.")
    print(f"\nApplied. Model {after['model']}, {len(after.get('tools') or [])} tools.")


if __name__ == "__main__":
    main()
