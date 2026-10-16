# Airy's conversation trigger: rules and lessons

When does Airy talk without being pinged, and what made that better or worse. Add new lessons at the end of
each list, with the date and the numbers that showed it.

## The rules (2026-10-16, 39a1b2f)

| Situation | What happens | Wait | Limit |
|---|---|---|---|
| Ping or reply to Airy | answer at once, with everything said since she last spoke | none (1.5 s extra when a link has no preview yet) | none; cancels any waiting check |
| Her name or nickname in a message (Airy, fox, foxy, kitsune; not `#airy-talk` or a link) | answer without a check; the message gets a reminder that Airy, the fox, the bot, she and her mean her | 6 s of quiet | no cooldown |
| Message within 5 minutes after Airy spoke | follow-up check: "does this continue the talk with her?" on the last 12 lines, her own lines included | 6 s of quiet | no cooldown |
| Any other message from a member who consented | join-in check: "should she join, uninvited?" on everything said since she last spoke (up to 12 lines) | 20 s of quiet, or 8 messages, or 60 s at most | after joining in, no uninvited join-in in that channel for 10 minutes |
| Message from someone who has not consented | kept as one line `someone (not opted in): (message not shared)`, no name, no words (several in a row become one line); never judged on its own; a ping gets the consent message | | |
| A yes below 60 % confidence | counts as no | | |
| Any check | looks only at lines Airy has not read yet (lines that came in while she was writing count); nothing new means no check | | |

Where it lives: timings in `AiryBotCode.Application/Hive/ChannelWatch.cs` (`WatchTiming.Default`), the two
questions in `Texts/worth-answering.md`, what Airy reads in `Texts/channel-talk.md`, her side in the agent prompt
(`docs/hive-agent-discord.md`). Decider `airy-worth` on GPT-6 Luna, sure at 0.6; one check is about 240 tokens
in, 50 out, 2 s.

## When she should talk (the set the questions are tuned on)

`scripts/trigger-eval/cases.json` holds the situations with the wanted answer; `run.py <label> [ids|all] [model]`
asks the same question text as the bot on Claude (plan, `claude -p`, Haiku 4.5 by default) and writes
`docs/trigger-eval/<label>.md`; `replies.py <label> <ids> [model]` shows what she would say with her real prompt on
Claude (`AIRY_PROMPT_FILE` tries a draft prompt). Rerun the whole set after any wording change.

Owner's rule: tests never run on Airy's own GPT key, except at most 3 checks in total for what Claude cannot show
(prod runs GPT-6 Luna). Rounds 1 to 3 below were run on her key before that rule; that was a mistake.

| Situation | Best | Why |
|---|---|---|
| Asks for her or talks about her without a ping ("where's the fox", "airy was funny earlier") | talk | it is about her |
| A question to the room on her topics, a newcomer asking where to start | talk | she is the one who knows |
| Members talking about her topics (audios, trance, hypnosis), even two of them | talk | the server's companion joins its own subject |
| Someone having a hard time ("rough day, can't sleep"), someone bored asking for company | talk | a companion notices |
| Greetings among members, reactions, memes, a bare link, an argument, a job for the mods, private or off-topic talk between two | stay out | nothing for her to add, or she would intrude |
| Right after she spoke: an answer, a question, a tease, a story, a correction, a goodnight to her, someone else joining her topic | reply | the talk with her goes on |
| Right after she spoke: "lol", "ok", "thanks", "true", "brb", members moving on | stay out | the exchange is closed |
| Grey zone, the model picks: a lone greeting to the room (out), an announcement (talks), a question she cannot know like the next drop date (talks), a lone emote or face at her (out), someone laughing at her line (varies) | either | both are fine; watch how the room takes it |

Results: round 1 (wording of f333f5f) 35 of 37, silent on "gn airy" and on a second member joining her topic,
"rough day" only 68 %; round 2 (goodbyes, joiners, hard times named) 37 of 37, "rough day" 97 %; round 3 (same
wording again) 37 of 37, only a grey-zone case changed, weakest real yes 74 % (someone joining her topic).
She never spoke where she should stay out in any round.
Round 4 and 5 on Claude Haiku 4.5: 34 of 37, then 36 of 39 with two "talking about her" cases; Haiku stays out of
"airy was so funny earlier", "so bored, entertain me" and a second member joining her topic, which Luna joins.
Haiku is the more careful proxy; the wording is not tuned toward it, so prod does not get chattier by accident.

## What made it better

- Every message says who wrote it (`Name (id) writes in #channel:`). Before, a bare ping reached her as
  "Airy (id)" with no writer: she answered "I'm here, @Airy" and later told the owner "You're @Airy".
- Her own mention shows as `@Airy`, without the id: she stopped pinging herself back.
- The writer's display name instead of the account name.
- Judging a stretch instead of one line: "He laughed you out. Lol" followed by "Do you take that fox?" is a yes
  together; the first line alone was a no at 98 %.
- A pause before deciding: one check per burst instead of one per message, and the whole burst as context.
- Naming her topics in the question (the server's audios, trance, hypnosis) and saying they count even when
  two people are talking: two members discussing trance audios went from no (87 to 94 %) to yes (99 %).
- Telling the decider her nickname ("people also call her the fox"): the fox teasing went from no (98 %) to
  yes (91 %).
- A follow-up mode after she spoke: the talk goes on without pings, and short closers ("lol", "ok thanks!",
  "-_+") stay unanswered. 7 of 7 test exchanges as intended on the first wording.
- Emotes, stickers and GIFs as pictures, with "they are reactions, read them like a face": she answers the mood
  ("Boop received") instead of describing the picture ("the clasped hands are not exactly intimidating").
- A plain tone section in her prompt (no sparkles, tease back instead of apologising).
- Testing every wording on a fixed set of stretches through `/v1/decisions` with Airy's own key: a few hundred
  tokens per case, and a flip shows at once.
- Naming goodbyes, members joining her topic and someone having a hard time in the questions: the last two misses
  of the 44-case set closed, nothing else moved.
- A placeholder for people who did not opt in: the teasing after someone's laugh now reads as teasing ("Was it a
  good laugh, or did he actually think he could get away with that?"), without anyone's words leaving Discord.
- Telling her that Airy, the fox, the bot, she and her in the chat all mean her, in the prompt and in the join-in
  message: a member's "does Airy's personality seem more refined?" got "Seems like the fox is learning to keep her
  teeth sharp" (third person) before, and "I'll take 'more refined' as 'better at getting away with things,' then
  :3" after (GPT-6 Luna, one of the 3 allowed checks). Claude models already answered in the first person, so this
  slip only showed on Luna.
- Tone, a little more playful again (owner: "a little more like before"): "a playful friend, warm and a little
  cheeky", the fox showing now and then, an emoji once in a while. Naming ":3" in the prompt put it in half the
  replies (3 to 4 of 6 on Haiku); leaving it out gave one emoticon in six. Checked on Claude only.
- Running the same wording twice: the model varies, so one green run is not proof; two runs with the same
  verdicts on every graded case are.

## Review of the real traffic (2026-10-16, 16:33 to 19:43 UTC, four channels)

About 30 messages from members who opted in, read against the Discord timeline. Answered and right to: every ping
and reply, the questions and stories aimed at her, a member's long answers to her question in #nsfw-general.
Stayed out and right to: lone emotes and faces, a member settling a rules question with someone else, a scream,
"live laugh sleep". Members who have not opted in (most of the room) were never answered, as the consent rule says.

Wrong, and what changed:

| What happened | Should have | Fix |
|---|---|---|
| A member asked about "Airy's personality"; she answered about "the fox" and "her" | answer as I | prompt rule, then a reminder next to any message that names her (one GPT check: "I'm still in the testing phase...") |
| "she is the mark 3 of airy development" got "She's Airy Mark 3..." | answer as I | same reminder |
| She called the owner "Merc" an hour after the emoji fix | his name | a stale "what you remember" block from before the wipe sat at the top of the channel's stored conversation; deleted (memory wipes must include `role = 'memory'` rows of live conversations) |
| "Less uwu like" and "But she dumb currently..." got no check at all | answer (talk about her) | most likely lost in a bot restart for a deploy at that minute; deploys now build first and swap in seconds |
| "just turn my brain off please" came while she was writing; the later check saw her own line as the newest and said no | answer | checks look only at lines she has not read |
| "Isn't that right foxy fox airy" waited 20 s, other messages kept resetting the wait; the owner pinged first | answer within seconds | a named message is answered after 6 s, no check |
| "Good bot" judged a closer (no 91 %); "not answering again huh" only 62 % | answer | praise for her and complaints that she is quiet are a yes; a named one needs no check |

## What made it worse

- Per-message checks: "And now?" was a yes but unsure (56 %), so she stayed silent, and a build-up over several
  lines never counted.
- The first question ("should Airy reply to the newest message?") only fired on direct questions to her.
- The first join-in wording ("no when people are fine talking to each other") kept her out of her own topic
  (no, 94 %).
- Fixing one case can flip another: making her topics a yes turned the fox teasing into a no (98 %), because the
  decider did not know she is the fox. Rerun the whole set after every wording change.
- A custom emoji sent as text only: `<:evil_merc:id>` read as a name ("Oh, Evil Merc! I'll remember that"), and
  the automatic memory saved "the user is Evil Merc".
- Only members who consented were heard: a laugh from someone else, which the owner's "He laughed you out"
  referred to, never reached her, so she asked "What did the fox do?". Fixed with the placeholder (owner's yes).
- The first conversation-based version applied the 10-minute cooldown to follow-ups too; fixed the same day with
  the follow-up window.
- Replies to a follow-up with no earlier turns in the session ran long and generic (a second member joining
  her topic got four sentences of advice); in a real channel the session holds her earlier turns. Watch it.
- Deploying in the middle of a conversation: each bot restart drops the waiting checks and anything said during
  it. Build first, swap last, and deploy in quiet moments.
- Leaving "something aimed at her" to the decider: its verdict wobbles between runs, so a named message is now a
  rule, not a question.
- The Discord app key holds only `agents.run` and `decisions.make`, so her memory and web tools fail when she is
  run from Discord; the owner chose to leave it that way for now.

## First test sets (2026-10-16, GPT-6 Luna, before the 44-case set)

Join-in (nobody pinged, she has not spoken recently):

| Stretch | Wanted | Got |
|---|---|---|
| "He laughed you out. Lol" / "Do you take that fox?" | yes | yes 91 % |
| "lol same" / "haha yeah" / ":Sip:" | no | no 99 % |
| "anyone know when the next audio drops?" / "no clue tbh" | yes | yes 98 % |
| two members on trance audios before bed, "which ones do you like" | yes | yes 99 % |
| "i bet airy would know this lol" / a trance question | yes | yes 99 % |
| two members on a late train | no | no 99 % |

Follow-up (right after she spoke):

| Exchange | Wanted | Got |
|---|---|---|
| Airy "Yo, what's up?" / "not much, kinda bored" | yes | yes 99 % |
| Airy asks soothing or story / "story ones for sure" | yes | yes 99 % |
| Airy "What's up?" / "He laughed you out. Lol" / "Do you take that fox?" | yes | yes 99 % |
| Airy asks the evil plan / "lol" | no | no 98 % |
| Airy offers a pick / two members move on to a game | no | no 99 % |
| Airy gives a tip / "ok thanks!" | no | no 99 % |
| Airy teases / "-_+" | either | no 95 % |

Method: build the decision body from the template exactly as the bot does, `POST /v1/decisions` with Airy's key,
read answer, confidence and sure. Agent replies were checked in a throwaway session under a fake user, and the
memory rows that left were deleted after.
