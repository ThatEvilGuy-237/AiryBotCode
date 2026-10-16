import json
import os
import re
import subprocess
import sys
import urllib.request
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
TALK = ROOT / "AiryBotCode.Application/Hive/Texts/channel-talk.md"
CASES = json.loads(Path(__file__).with_name("cases.json").read_text())
SPINE = "http://127.0.0.1:7000"
WORKSPACE = "60376c50-60b8-458d-aee9-e0a8d4414624"
AGENT = "2d7b492e-27f2-49e0-8261-c9330aa3df60"
FAKE_USER = "trigger-eval"


def sections():
    found, current = {}, None
    for line in TALK.read_text().split("\n"):
        if line.startswith("## "):
            current = line[3:].strip()
            found[current] = []
        elif current:
            found[current].append(line)
    return {k: "\n".join(v).strip() for k, v in found.items()}


def fill(template, values):
    return re.sub(r"\{\{(\w+)\}\}", lambda m: values.get(m.group(1), m.group(0)), template).strip()


def message(case, sec):
    lines = case["lines"]
    author = f"{lines[-1][0]} (100000000000000001)"
    channel = case.get("channel", "airy-talk")
    if case["kind"] == "join-in":
        return fill(sec["joining in"], {"bot": "Airy", "channel": channel, "author": author,
                                        "lines": "\n".join(f"{a}: {t}" for a, t in lines)})
    since = [(a, t) for a, t in lines[:-1]]
    last_bot = max((i for i, (a, _) in enumerate(since) if a == "Airy"), default=-1)
    since = since[last_bot + 1:]
    if not since:
        return fill(sec["alone"], {"author": author, "channel": channel, "message": lines[-1][1]})
    return fill(sec["with context"], {"bot": "Airy", "channel": channel, "author": author,
                                      "lines": "\n".join(f"{a}: {t}" for a, t in since), "message": lines[-1][1]})


def run(case, sec, token):
    payload = {"message": message(case, sec), "sessionId": f"trigger-eval-{case['id']}", "userId": FAKE_USER}
    request = urllib.request.Request(f"{SPINE}/agents/{AGENT}/run", data=json.dumps(payload).encode(), method="POST",
                                     headers={"content-type": "application/json", "Authorization": f"Bearer {token}",
                                              "X-Hive-Workspace": WORKSPACE})
    with urllib.request.urlopen(request, timeout=120) as response:
        return json.load(response).get("text")


def clean():
    sql = f"""begin;
delete from memory.preformed_task_conversations where conversation_id in (select id from memory.conversations where session_id like 'trigger-eval-%');
delete from memory.context_maps where conversation_id in (select id from memory.conversations where session_id like 'trigger-eval-%');
delete from memory.messages where conversation_id in (select id from memory.conversations where session_id like 'trigger-eval-%');
delete from memory.conversations where session_id like 'trigger-eval-%';
delete from memory.bm25_facts where access='{FAKE_USER}';
delete from memory.summary_entries where access_key='{FAKE_USER}';
delete from memory.image_assets where user_id='{FAKE_USER}';
commit;"""
    subprocess.run(["docker", "exec", "airy-postgres-db", "psql", "-U", "airy_user", "-d", "hive_cookie_cream", "-tAc", sql],
                   capture_output=True, text=True, check=True)


def main():
    label, ids = sys.argv[1], sys.argv[2].split(",")
    token = Path(os.environ["SPINE_TOKEN_FILE"]).read_text().strip()
    sec = sections()
    rows = []
    try:
        for case in [c for c in CASES if c["id"] in ids]:
            said = run(case, sec, token)
            last = case["lines"][-1]
            rows.append(f"| {case['id']} | {last[0]}: {last[1]} | {said} |")
            print(f"{case['id']}: {said}")
    finally:
        clean()
    out = ROOT / "docs/trigger-eval" / f"{label}.md"
    out.write_text(f"# {label}\n\n| Case | Last line | Airy |\n|---|---|---|\n" + "\n".join(rows) + "\n")


if __name__ == "__main__":
    main()
