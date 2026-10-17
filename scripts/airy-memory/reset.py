"""Wipe what Airy remembers in the prod Hive (tenant database, memory schema), after a backup.

Without filters: everything she remembers (facts, conversations including their "what you remember" blocks, notes,
graph, pictures, extraction logs); her memory catchers, note banks and graph bank stay.
--session PATTERN: only conversations whose session id matches (SQL LIKE), with their messages.
--user ID: only facts, notes and pictures kept for that user.
Dry run by default: counts what would go. --yes backs up and deletes in one transaction.
"""
import argparse
import datetime
import subprocess
import sys
from pathlib import Path

DB = "hive_cookie_cream"
CONTENT = ["preformed_task_conversations", "messages", "context_maps", "conversations", "bm25_facts", "summary_entries",
           "graph_edges", "graph_nodes", "image_assets", "memory_catcher_traces", "task_captures", "preformed_tasks",
           "consolidation_ops", "consolidation_runs"]
RESTART = ["hive-core-obsidian-mind-1", "airybotcode-airydevbot-1"]


def psql(query, single=True):
    command = ["docker", "exec", "airy-postgres-db", "psql", "-U", "airy_user", "-d", DB, "-v", "ON_ERROR_STOP=1"]
    command += ["-tAc", query] if single else ["-c", query]
    return subprocess.run(command, capture_output=True, text=True, check=True).stdout.strip()


def quote(value):
    return "'" + value.replace("'", "''") + "'"


def plan(session, user):
    if not session and not user:
        counts = {t: psql(f"select count(*) from memory.{t}") for t in CONTENT}
        return counts, f"begin; truncate {', '.join('memory.' + t for t in CONTENT)}; commit;"
    steps, counts = [], {}
    if session:
        chosen = f"(select id from memory.conversations where session_id like {quote(session)})"
        for table in ["preformed_task_conversations", "context_maps", "messages"]:
            counts[table] = psql(f"select count(*) from memory.{table} where conversation_id in {chosen}")
            steps.append(f"delete from memory.{table} where conversation_id in {chosen};")
        counts["conversations"] = psql(f"select count(*) from memory.conversations where session_id like {quote(session)}")
        steps.append(f"delete from memory.conversations where session_id like {quote(session)};")
    if user:
        for table, column in [("bm25_facts", "access"), ("summary_entries", "access_key"), ("image_assets", "user_id")]:
            counts[table] = psql(f"select count(*) from memory.{table} where {column} = {quote(user)}")
            steps.append(f"delete from memory.{table} where {column} = {quote(user)};")
    return counts, "begin; " + " ".join(steps) + " commit;"


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--session")
    parser.add_argument("--user")
    parser.add_argument("--yes", action="store_true", help="back up, then delete")
    parser.add_argument("--restart", action="store_true", help="restart the memory service and the bot afterwards")
    args = parser.parse_args()
    counts, statement = plan(args.session, args.user)
    for table, count in counts.items():
        print(f"{table:32} {count}")
    if not args.yes:
        print("\nDry run. Run again with --yes to back up and delete.")
        return
    folder = Path.home() / "backups" / f"airy-memory-{datetime.datetime.now(datetime.timezone.utc):%Y-%m-%dT%H%M%SZ}"
    folder.mkdir(parents=True)
    with open(folder / f"{DB}-memory.dump", "wb") as dump:
        subprocess.run(["docker", "exec", "airy-postgres-db", "pg_dump", "-U", "airy_user", "-d", DB, "-n", "memory", "-Fc"],
                       stdout=dump, check=True)
    print(f"\nBackup: {folder} (restore with pg_restore -n memory)")
    psql(statement, single=False)
    print("Deleted.")
    if args.restart:
        subprocess.run(["docker", "restart", *RESTART], check=True, capture_output=True)
        print("Restarted: " + ", ".join(RESTART))


if __name__ == "__main__":
    sys.exit(main())
