---
title: Airy's conversation code, rework plan
status: in progress 2026-10-17
---

# Airy's conversation code, rework plan

On 2026-10-16 Airy's Discord conversation behaviour was built in 18 quick commits (4f138f5 to 39a1b2f): who writes,
pictures for emotes and GIFs, the conversation-based check with a pause and a cooldown, follow-ups, a placeholder for
people who did not opt in, unread lines, named messages answered without a check, and a reminder that "Airy" means
her. Each fix was tested and deployed on its own. The owner asked on 2026-10-17 to go through all of it and give it a
proper structure. Behaviour stays as it is on prod, except for the bugs listed below.

## What is there

| Piece | Where | Job |
|---|---|---|
| `MessageSendHandler` | Infrastructure, 386 lines | old webhook forwarding, the whole Airy-channel flow (two static methods of 70 lines), reading a Discord message into text, consent message, splitting and sending replies |
| `ChannelTranscript` + `HeardLine` + `ChannelTalk` | Application/Hive | the last 30 lines per channel, the read marker, and the text Airy gets |
| `ChannelWatch` | Application/Hive | the pause before a check, "in talk", cooldown |
| `AiryAnswers.cs` | Application/Hive, 339 lines | answer modes, Hive connection, verdict, answer rules, the check question, the markdown text loader, the decider client, the connection check, the old agent-link migration |
| `ChatPictures`, `AiryNames`, `ImageAttachments` | Application/Hive | emotes, stickers, GIFs and links as pictures; her names |
| Texts | `channel-talk.md`, `worth-answering.md`, `hive-messages.md` | every sentence the agent and the decider read |
| Prompt | prod Hive only (`spine.agent`), a copy of one section in `docs/hive-agent-discord.md` | changed by hand-written scripts six times |
| Tests | xunit for the pieces; `scripts/trigger-eval` for the questions (Claude) | the flow itself (when to answer, when to check, cooldown, read marker) has no test |

## What is wrong with how it is put together

1. The flow is untested: the rules that decide between "answer now", "wait and check" and "stay quiet" live in two
   static methods inside a Discord event handler, mixed with Discord calls.
2. The ping path and the check path repeat the same work (connection, pictures, run, send).
3. Per-channel state is split over two singletons (lines and the read marker in one, "spoke", cooldown and the timer
   in the other) that must be kept in step by hand.
4. Names drifted: worth, decide, join in, follow-up, targeted, addressed, invited, named, heard, unheard, not shared,
   seen.
5. `AiryAnswers.cs` is a grab bag, and the markdown loader that every text uses lives in the decider question class.
6. Limits are scattered: 30 lines kept, 20 and 4000 for context, 12 and 300 for the question, 20 s, 8, 60 s, 10 min,
   6 s, 5 min, 4 pictures, 1.5 s for link previews.
7. Bugs found while reading:
   - two runs can be in flight in one channel (a follow-up check fires while the answer to a ping is still being
     written), so she can answer twice at once;
   - an error in the Airy-channel path is logged as "[Webhook] message forward error";
   - prompt edits are hand-written scripts with no diff and no record in git;
   - a memory wipe by hand missed the `role = 'memory'` rows inside a live conversation (the "Merc" bug).

## Words (used in code, texts and docs from now on)

| Word | Meaning |
|---|---|
| ping | a Discord mention of Airy or a reply to one of her messages |
| named | her name or a nickname (Airy, fox, foxy, kitsune) in a message, without a ping |
| member | someone who opted in; only members are heard and answered |
| channel log | the last 30 lines of a channel: members' lines, Airy's own, and one placeholder line for people who did not opt in |
| read | lines Airy has been given in a run; later lines are unread |
| in talk | Airy spoke in the channel in the last 5 minutes |
| check | one question to the Hive decider about the unread lines: a join-in check (not in talk) or a follow-up check (in talk) |
| cooldown | after joining in uninvited, 10 minutes without uninvited join-ins in that channel |

## Target layout

`AiryBotCode.Application/Conversation/` (Discord-free, all of it testable):

| Type | Job |
|---|---|
| `ConversationSettings` | every limit and wait above, one record with the prod defaults |
| `ChatMessage` | an incoming Discord message as plain data: ids, channel, writer's display name, readable text, pictures, ping or not |
| `IChatChannel` | what the flow needs from Discord: typing, send a reply, look again for link previews, ask for consent |
| `ChannelLog` | the channel log and the read marker (was `ChannelTranscript`) |
| `ChannelWatch` | pause, in talk, cooldown (as now) |
| `ConversationRules` | pure: what to do with a message (stay quiet, hear only, answer now, ask consent, wait for a check, wait shortly) and what kind of check to run |
| `AiryConversation` | the flow: hear, decide, check, answer; one run at a time per channel |
| `AgentMessage` | the text the agent gets (was `ChannelTalk`) |
| `JoinCheck` | the decider question and its verdict (was `WorthQuestion` and `WorthVerdict`) |
| `Texts` | loads the markdown sections and fills them |
| `ChatPictures`, `AiryNames` | as now |

Infrastructure keeps only Discord: `DiscordChatMessages` maps a `SocketMessage` to a `ChatMessage`, `DiscordChatChannel`
implements `IChatChannel`, and `MessageSendHandler` calls `AiryConversation` and keeps the old webhook forwarding apart.

Prompt: the two sections that belong to the bot's behaviour (how she talks, Discord) live in `docs/conversation/prompt/`
and `scripts/airy-prompt/apply.py` puts them into the live prompt with a diff first; her persona, memory and continuity
stay in the Hive. A `scripts/airy-memory/reset.py` makes the backup and wipes everything, the `role = 'memory'` rows
included.

## Steps

| # | Step | Done when |
|---|---|---|
| 1 | Characterisation tests: the exact agent messages and check bodies for a set of inputs, taken from the current code | tests green on the current code |
| 2 | `Texts`, `ConversationSettings`, new names, files moved into `Conversation/` | all tests green, same strings |
| 3 | `ChatMessage`, `IChatChannel`, Discord mapper and channel | handler builds, tests green |
| 4 | `ConversationRules` and `AiryConversation`, one run at a time per channel; handler thin | flow tests with fakes: ping, named, follow-up, join-in, cooldown, unread lines, consent, not listening, two runs |
| 5 | Prompt sections and apply script; memory reset script | dry run shows no diff against prod |
| 6 | Docs: one design page, lessons, the review | links checked |
| 7 | Fresh review by a reviewer agent; fix what holds | no open finding |
| 8 | Deploy at a quiet moment, check live | bot up, one real exchange looks right |
