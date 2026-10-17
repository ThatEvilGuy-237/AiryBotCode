# Lessons: what made Airy's conversation better or worse

From building it on 2026-10-16 and the rework on 2026-10-17. Add new lessons at the end of a list, with the date and
the numbers that showed it. Design: [README.md](README.md).

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
- Testing every wording on a fixed set of stretches: a few hundred tokens per case, and a flip shows at once. The
  first rounds ran through Airy's own GPT key, which the owner then ruled out; they run on Claude now.
- Naming goodbyes, members joining her topic and someone having a hard time in the questions: the last two misses
  of the 44-case set closed, nothing else moved.
- A placeholder for people who did not opt in: the teasing after someone's laugh now reads as teasing ("Was it a
  good laugh, or did he actually think he could get away with that?"), without anyone's words leaving Discord.
- Telling her that Airy, the fox, the bot, she and her in the chat all mean her, in the prompt and in the join-in
  message: a member asking whether her personality felt more refined got "Seems like the fox is learning to keep her
  teeth sharp" (third person) before, and "I'll take 'more refined' as 'better at getting away with things,' then
  :3" after (GPT-6 Luna, one of the 3 allowed checks). Claude models already answered in the first person, so this
  slip only showed on Luna.
- Tone, a little more playful again (owner: "a little more like before"): "a playful friend, warm and a little
  cheeky", the fox showing now and then, an emoji once in a while. Naming ":3" in the prompt put it in half the
  replies (3 to 4 of 6 on Haiku); leaving it out gave one emoticon in six. Checked on Claude only.
- Running the same wording twice: the model varies, so one green run is not proof; two runs with the same
  verdicts on every graded case are.

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

## Lessons that carry to any bot that chimes in

- Judge the conversation, not the newest line, and wait for a pause so a burst is one check.
- Always say who wrote a message, and show the bot's own mention without its id.
- Put the bot's topics and nicknames into the decider question; the decider knows nothing about the persona.
- Something aimed at the bot by name is a rule, not a question: verdicts wobble between runs.
- A reply right after the bot spoke is a follow-up, with its own question and no cooldown.
- Keep a fixed set of situations with wanted answers and rerun all of it, twice, after every wording change.
- Pictures for emotes and GIFs, framed as reactions, or the model narrates the image or reads the emoji name as a person.
- A gap in the context hurts more than a wording: one placeholder line fixed a whole misunderstanding.
- Keep the flow out of the chat-event handler: the rules became testable only once they were pure functions.
