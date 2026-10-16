# Airy's conversation trigger: rules and lessons

When does Airy talk without being pinged, and what made that better or worse. Add new lessons at the end of
each list, with the date and the numbers that showed it.

## The rules (2026-10-16, f333f5f)

| Situation | What happens | Wait | Limit |
|---|---|---|---|
| Ping or reply to Airy | answer at once, with everything said since she last spoke | none (1.5 s extra when a link has no preview yet) | none; cancels any waiting check |
| Message within 5 minutes after Airy spoke | follow-up check: "does this continue the talk with her?" on the last 12 lines, her own lines included | 6 s of quiet | no cooldown |
| Any other message from a member who consented | join-in check: "should she join, uninvited?" on everything said since she last spoke (up to 12 lines) | 20 s of quiet, or 8 messages, or 60 s at most | after joining in, no uninvited join-in in that channel for 10 minutes |
| Message from someone who has not consented | not heard, not judged; a ping gets the consent message | | |
| A yes below 60 % confidence | counts as no | | |

Where it lives: timings in `AiryBotCode.Application/Hive/ChannelWatch.cs` (`WatchTiming.Default`), the two
questions in `Texts/worth-answering.md`, what Airy reads in `Texts/channel-talk.md`, her side in the agent prompt
(`docs/hive-agent-discord.md`). Decider `airy-worth` on GPT-6 Luna, sure at 0.6; one check is about 240 tokens
in, 50 out, 2 s.

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
- Only members who consented are heard: a laugh from someone else, which the owner's "He laughed you out"
  referred to, never reached her, so she asked "What did the fox do?". Open, the owner's call.
- The first conversation-based version applied the 10-minute cooldown to follow-ups too; fixed the same day with
  the follow-up window.
- The Discord app key holds only `agents.run` and `decisions.make`, so her memory and web tools fail when she is
  run from Discord; the owner chose to leave it that way for now.

## Test sets (2026-10-16, GPT-6 Luna, final wording)

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
