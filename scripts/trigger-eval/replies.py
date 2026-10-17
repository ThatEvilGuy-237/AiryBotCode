import importlib.util
import json
import os
import re
import subprocess
import sys
from pathlib import Path

from run import claude

ROOT = Path(__file__).resolve().parents[2]
TALK = ROOT / "AiryBotCode.Application/Conversation/Texts/agent-message.md"
CASES = json.loads(Path(__file__).with_name("cases.json").read_text())
AGENT = "2d7b492e-27f2-49e0-8261-c9330aa3df60"
MODEL = "claude-sonnet-5-5"


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


def prompt_from_git():
    spec = importlib.util.spec_from_file_location("apply", ROOT / "scripts/airy-prompt/apply.py")
    apply = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(apply)
    return apply.merge(persona(), apply.SECTIONS)


def persona():
    sql = f"select system_prompt from spine.agent where id='{AGENT}'"
    out = subprocess.run(["docker", "exec", "airy-postgres-db", "psql", "-U", "airy_user", "-d", "hive_cookie_cream", "-tAc", sql],
                         capture_output=True, text=True, check=True)
    return out.stdout.strip()


def main():
    label, ids = sys.argv[1], sys.argv[2].split(",")
    model = sys.argv[3] if len(sys.argv) > 3 else MODEL
    sec = sections()
    system = Path(os.environ["AIRY_PROMPT_FILE"]).read_text() if os.environ.get("AIRY_PROMPT_FILE") else prompt_from_git()
    rows = []
    for case in [c for c in CASES if c["id"] in ids]:
        said = (claude(system, message(case, sec), model).get("result") or "").strip()
        last = case["lines"][-1]
        rows.append(f"| {case['id']} | {last[0]}: {last[1]} | {said} |")
        print(f"{case['id']}: {said}")
    out = ROOT / "docs/trigger-eval" / f"{label}.md"
    out.write_text(f"# {label} ({model})\n\n| Case | Last line | Airy |\n|---|---|---|\n" + "\n".join(rows) + "\n")


if __name__ == "__main__":
    main()
