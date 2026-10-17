import json
import re
import subprocess
import sys
from concurrent.futures import ThreadPoolExecutor
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
TEXTS = ROOT / "AiryBotCode.Application/Conversation/Texts/join-check.md"
CASES = Path(__file__).with_name("cases.json")
MODEL = "claude-haiku-4-5-20251001"
SURE_AT = 0.6
SCHEMA = json.dumps({"type": "object", "properties": {"answer": {"type": "boolean"}, "confidence": {"type": "number"}},
                     "required": ["answer", "confidence"]})
DECIDER = "You answer one yes/no question about a Discord channel. Read the state, answer the question, and say how sure you are from 0 to 1."
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
        "state": fill(sec["follow-up state" if follow else "join-in state"], values),
        "questions": {"worth": {"type": "yesno", "instructions": fill(sec["follow-up" if follow else "join-in"], values)}},
        "user": "discord:100000000000000001",
    }


def claude(system, prompt, model, schema=None):
    command = ["claude", "-p", "--model", model, "--tools", "", "--setting-sources", "", "--strict-mcp-config",
               "--no-session-persistence", "--output-format", "json", "--system-prompt", system]
    if schema:
        command += ["--json-schema", schema]
    out = subprocess.run(command + [prompt], capture_output=True, text=True, timeout=180, cwd="/tmp")
    return json.loads(out.stdout)


def ask(case, sec, model):
    b = body(case, sec)
    prompt = f"Question: {b['questions']['worth']['instructions']}\n\nState:\n{b['state']}"
    answer = claude(DECIDER, prompt, model, SCHEMA).get("structured_output") or {}
    yes = answer.get("answer") is True
    confidence = answer.get("confidence") or 0
    sure = confidence >= SURE_AT
    speaks = yes and sure
    want = case["want"]
    ok = want == "either" or (want == "yes") == speaks
    return {"id": case["id"], "kind": case["kind"], "want": want, "yes": yes, "sure": sure,
            "confidence": confidence, "speaks": speaks, "ok": ok}


def main():
    label = sys.argv[1] if len(sys.argv) > 1 else "run"
    only = set(sys.argv[2].split(",")) if len(sys.argv) > 2 and sys.argv[2] != "all" else None
    model = sys.argv[3] if len(sys.argv) > 3 else MODEL
    sec = sections()
    cases = [c for c in json.loads(CASES.read_text()) if not only or c["id"] in only]
    with ThreadPoolExecutor(4) as pool:
        results = list(pool.map(lambda c: ask(c, sec, model), cases))
    graded = [r for r in results if r["want"] != "either"]
    passed = sum(r["ok"] for r in graded)
    false_yes = [r["id"] for r in graded if r["speaks"] and r["want"] == "no"]
    false_no = [r["id"] for r in graded if not r["speaks"] and r["want"] == "yes"]
    rows = ["| Case | Kind | Want | Got | OK |", "|---|---|---|---|---|"]
    for r in results:
        got = f"{'yes' if r['yes'] else 'no'} {round((r['confidence'] or 0) * 100)} %{'' if r['sure'] else ', unsure'}"
        rows.append(f"| {r['id']} | {r['kind']} | {r['want']} | {got} | {'✓' if r['ok'] else '✗'} |")
    summary = f"{label} ({model}): {passed} of {len(graded)} as wanted; speaks when it should not: {', '.join(false_yes) or 'none'}; silent when it should speak: {', '.join(false_no) or 'none'}"
    print(summary)
    print("\n".join(rows))
    out = ROOT / "docs/trigger-eval" / f"{label}.md"
    out.write_text(f"# {label}\n\n{summary}\n\n" + "\n".join(rows) + "\n")


if __name__ == "__main__":
    main()
