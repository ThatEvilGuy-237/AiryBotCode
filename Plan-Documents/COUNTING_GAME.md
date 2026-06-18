# Counting Game — Plan & Design

A counting game for Airy: members count up in a channel; numbers may be plain
(`12`) or **math expressions** that evaluate to the next number (`6*2`, `sqrt(144)`).
Success is a silent ✅ react; failure and "mini-boss" moments are **voiced by Airy
through the Hive**. Mini-bosses are AI-generated math puzzles, with the actual math
done by a **containerised Math MCP server** plugged into Wraith-Worker.

This spans two repos:
- **AiryBotCode** — the counting engine, the answer evaluator, the boss state
  machine, and the triggers to the Hive.
- **The-Hive** — the `math-mcp` container (mathjs) wired into Wraith-Worker, plus
  Airy handling the `counting_*` triggers and generating boss puzzles.

---

## 1. Core counting

- One counting channel per bot (`CountingChannelId`, `0` = off — opt-in, like the
  spam catcher's role).
- Next valid value = current + 1, starting at `StartNumber` (1).
- A user can't count twice in a row (`AllowSameUserTwice` = false).
- Messages that don't parse as a number/expression are **ignored** (chatter is
  allowed in the channel), so only a *wrong* count breaks the chain.
- Success → react ✅ only (no message). Track a high score.
- Failure → reset to `StartNumber`, and announce via the Hive (below).

### Math answers (the "fun/complex" bit)

`CountingMath` — a self-contained, dependency-free evaluator on the **bot side**
(shunting-yard → RPN), used to validate player answers:
- operators `+ - * / % ^`, unary minus, parentheses, decimals;
- functions `sqrt abs floor ceil round pow log ln min max` and `!`/`factorial`;
- guardrails: input length cap, exponent/factorial caps (no `9^9999` DoS);
- a value counts if it rounds to the expected integer within `DecimalTolerance`
  (0.01, BetterCounting-style — floats from `sqrt`/`/` need slack).

Pure and unit-tested like `LevelMath` (`CountingMathTests`).

---

## 2. Failure & boss announcements — via the existing Hive socket

We **reuse the bot's existing Hive connection** — no new URL/token. The
`HiveEffectListener` already owns the live tools-WS and can send frames upstream
(that's how `ask_user` answers go back via `SendAnswerAsync`). `HiveEffectGateway`
is the singleton bridge any component resolves.

Plan:
- Extend `IHiveResponseSender` / listener / gateway with a generic
  `SendEventAsync(type, payload, sessionId)` — same plumbing as `SendAnswerAsync`,
  different frame `type` (`"counting_fail"`, `"counting_boss"`).
- The counting component resolves `HiveEffectGateway` and fires the event with
  `sessionId = countingChannelId`.
- Airy's reply (`say`) returns through the **existing** effect listener and
  `EffectRouter` (sessionId → channel) — no new delivery code.
- **Fallback:** if the gateway isn't connected, post the static `FailMessage` so it
  is never silent. The reset always happens locally regardless of the Hive.

---

## 3. Mini-bosses

At milestone counts, Airy spawns a math puzzle; the AI invents it, the **bot judges
the answers** (instant, no AI round-trip per guess).

Flow:
1. Count crosses a milestone → bot sends `counting_boss { channelId, milestone }`
   up the gateway.
2. **Hive:** Airy uses the Math MCP tool to build a formula + compute its exact
   answer, posts the boss via `say`, and returns the expected answer to the bot.
3. **Bot:** stores boss state (`BossActive`, `BossAnswer`) on the counting row;
   counting pauses. Player answers are validated with the same `CountingMath`
   (formulas allowed). First correct → ✅, clear boss, resume at milestone+1, and
   (optional) tell the Hive "defeated" so Airy can gloat.

### Milestone schedule

`BossMilestones` array setting, default `[10, 20, 50, 100, 200, 300, 400]`. Once the
array is exhausted, continue by the **last gap** (400−300 = **+100**) → 500, 600, …

---

## 4. The Math MCP (Hive side)

A standalone, containerised MCP server (`ExternalServices/math-mcp`, Bun + mathjs)
that Wraith-Worker connects to as an MCP toolbox over **streamable HTTP**.

- Tools:
  - `math_eval { expression }` → exact result (mathjs, hardened: `import` /
    `createUnit` / `evaluate` / `parse` disabled in the instance to prevent escapes).
  - `math_problem { difficulty?, target? }` → `{ formula, answer }`, a ready boss
    puzzle with an integer answer.
- Wiring: a `math-mcp` service on the Hive compose network; Wraith gets
  `MATH_MCP_URL=http://math-mcp:8787/mcp` and registers it in `TOOLBOX_DEFS`
  (`adapter: "mcp", transport: { kind: "http", url }`). Airy then sees `math_eval`
  / `math_problem` through Wraith automatically.

mathjs chosen for its sandboxed parser and broad function set
(https://github.com/josdejong/mathjs, https://mathjs.org/docs/expressions/parsing.html).

---

## 5. Settings (panel, hot-reloadable) — `x(base)`

Counting:
- `CountingChannelId`(0 = off)
- `AllowMath`(true) · `MathOnly`(false)
- `AllowSameUserTwice`(false)
- `StartNumber`(1) · `ResetOnWrong`(true)
- `ReactOnSuccess`(true)
- `DecimalTolerance`(0.01)
- `TrackHighScore`(true)
- `FailMessage`(fallback when the Hive is unreachable)

Bosses:
- `BossesEnabled`(true)
- `BossMilestones`(`[10,20,50,100,200,300,400]`, last gap repeats)
- `BossWrongResets`(false)

No Hive URL/token settings — the existing effects socket is reused.

---

## 6. Persistence

`CountingState` entity (EF, mirroring `LevelUser`): keyed `(BotId, ChannelId)` —
`CurrentCount, LastUserId, LastMessageId, HighScore, BossActive, BossAnswer,
UpdatedAt`. Table created via the `EnsureNewerTables` raw-SQL self-heal (no
migrations in this project).

---

## 7. Build order & status

1. **Math MCP container** + Wraith wiring (The-Hive) — *in progress*.
2. **Bot side**: `CountingMath`, counting engine, boss state machine, the
   `SendEventAsync` gateway addition, fallback (AiryBotCode).
3. **Hive glue**: `counting_*` trigger handling + Airy boss prompt (depends on 1 & 2).

Opt-in everywhere: disabled per-bot by default; prod `AiryBot`'s hardcoded action
list is unchanged.
