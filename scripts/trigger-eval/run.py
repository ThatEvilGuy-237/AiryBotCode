import json
import re
import subprocess
import sys
import urllib.request
from concurrent.futures import ThreadPoolExecutor
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
TEXTS = ROOT / "AiryBotCode.Application/Hive/Texts/worth-answering.md"
CASES = Path(__file__).with_name("cases.json")
SPINE = "http://127.0.0.1:7000/v1/decisions"
BOT_ID = "1437132076066406510"
MAX_RECENT = 12
MAX_LINE = 300


def sections():
    found, current = {}, None
    for line in TEXTS.read_text().split("\n"):
        if line.startswith("## "):
            current = line[3:].strip()
            found[current] = []
        elif current:
            found[current].append(line)
    return {k: "\n".join(v).strip() for k, v in found.items()}


def cut(text, limit=MAX_LINE):
    flat = text.replace("\n", " ").strip()
    return flat if len(flat) <= limit else flat[:limit] + "…"


def fill(template, values):
    return re.sub(r"\{\{(\w+)\}\}", lambda m: values.get(m.group(1), m.group(0)), template).strip()


def body(case, sec):
    lines = case["lines"]
    follow = case["kind"] == "follow-up"
    recent = [f"{a}: {cut(t)}" for a, t in lines[:-1]][-MAX_RECENT:]
    values = {
        "bot": "Airy",
        "channel": case.get("channel", "airy-talk"),
        "author": lines[-1][0],
        "message": cut(lines[-1][1], 2000),
        "recent": "\n".join(recent) if recent else sec["no recent"],
    }
    return {
        "state": fill(sec["follow-up state" if follow else "state"], values),
        "questions": {"worth": {"type": "yesno", "instructions": fill(sec["follow-up" if follow else "instructions"], values)}},
        "user": "discord:100000000000000001",
    }


def app_key():
    sql = f'select "HiveAppKey" from "BotSettings" where "BotId"={BOT_ID}'
    out = subprocess.run(["docker", "exec", "airy-postgres-db", "psql", "-U", "airy_user", "-d", "airy_db", "-tAc", sql],
                         capture_output=True, text=True, check=True)
    return out.stdout.strip()


def ask(case, sec, key):
    request = urllib.request.Request(SPINE, data=json.dumps(body(case, sec)).encode(), method="POST",
                                     headers={"content-type": "application/json", "X-Hive-App-Key": key})
    with urllib.request.urlopen(request, timeout=60) as response:
        answer = (json.load(response).get("answers") or {}).get("worth") or {}
    yes = answer.get("answer") is True
    sure = answer.get("sure") is True
    speaks = yes and sure
    want = case["want"]
    ok = want == "either" or (want == "yes") == speaks
    return {"id": case["id"], "kind": case["kind"], "want": want, "yes": yes, "sure": sure,
            "confidence": answer.get("confidence"), "speaks": speaks, "ok": ok}


def main():
    label = sys.argv[1] if len(sys.argv) > 1 else "run"
    only = set(sys.argv[2].split(",")) if len(sys.argv) > 2 else None
    sec, key = sections(), app_key()
    cases = [c for c in json.loads(CASES.read_text()) if not only or c["id"] in only]
    with ThreadPoolExecutor(4) as pool:
        results = list(pool.map(lambda c: ask(c, sec, key), cases))
    graded = [r for r in results if r["want"] != "either"]
    passed = sum(r["ok"] for r in graded)
    false_yes = [r["id"] for r in graded if r["speaks"] and r["want"] == "no"]
    false_no = [r["id"] for r in graded if not r["speaks"] and r["want"] == "yes"]
    rows = ["| Case | Kind | Want | Got | OK |", "|---|---|---|---|---|"]
    for r in results:
        got = f"{'yes' if r['yes'] else 'no'} {round((r['confidence'] or 0) * 100)} %{'' if r['sure'] else ', unsure'}"
        rows.append(f"| {r['id']} | {r['kind']} | {r['want']} | {got} | {'✓' if r['ok'] else '✗'} |")
    summary = f"{label}: {passed} of {len(graded)} as wanted; speaks when it should not: {', '.join(false_yes) or 'none'}; silent when it should speak: {', '.join(false_no) or 'none'}"
    print(summary)
    print("\n".join(rows))
    out = ROOT / "docs/trigger-eval" / f"{label}.md"
    out.write_text(f"# {label}\n\n{summary}\n\n" + "\n".join(rows) + "\n")


if __name__ == "__main__":
    main()
