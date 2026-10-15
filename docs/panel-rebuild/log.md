# Airy panel rebuild log

Newest first. Plan: `plan.md`.

## 2026-10-15 · R1 backend: one Hive connection, channels with an answer rule

- Bot settings carry the Hive connection (`HiveUrl`, `HiveAgentId`, `HiveAppKey`); table `AiryChannels`
  holds the channels Airy talks in with their rule (`always`, `mention`, `decide`) and the last decision.
- A "When it's worth it" channel asks the Hive `POST /v1/decisions` (question text in
  `Application/Hive/Texts/worth-answering.md`, last 6 cached messages as context) only for messages that
  do not mention Airy, from people who accepted the data notice; a sure yes answers, anything else stays
  quiet; a mention or reply is always answered.
- At start, agent-mode links move into the connection and `AiryChannels` (rule from the old
  RequireMention switch), then the link rows go. Rehearsed on a copy of prod's airy_db: first start moved
  1 link of Cookie&Creams (channel becomes `mention`, as RequireMention was on), second start moved
  nothing; the 3 sync webhooks stayed; `ChannelDecisionWatches` dropped.
- API: `api/channels`, `api/settings/{id}/hive` (get, put, delete, `agents`, `check`); service setup in
  `PanelServices` with the counting, level and channel repositories the API was missing (Status and
  Decision watches answered 500); a test builds every controller from it and fails on the old gap.
- Webhook edits keep the stored secret when it is left blank; webhooks are sync or async only.
- Decision watches (listener per watch) removed.
- Tests: 145 of 145.
- Not deployed yet: ships with the panel (R6).

## 2026-10-15 · R0 inventory and identity

Inventory of the old panel (findings in `plan.md`). Company `airy` in evil-ui (0d26c22): `@airy/ui` 0.1.0,
sky on navy, DM Sans and JetBrains Mono, cloud BrandMark. New app `AiryBotCode.Frontend/AiryPanel` with the
package and icons from `docs/brand/logo.svg`.
