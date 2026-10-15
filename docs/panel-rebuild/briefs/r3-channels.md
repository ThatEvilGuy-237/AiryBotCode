# UI brief: R3 Channels

| Slot | Decision | Rule ids applied |
|---|---|---|
| Surface | Operator tool, Airy identity (`@airy/ui`). Desktop first, phone must work. | kit, S13 |
| Usual case (one sentence) | The owner picks a channel by name and chooses when Airy answers there (always, when mentioned, or when the Hive says it is worth it), on the one Hive connection the bot already has. | S21 |
| Exception paths | Connect the Hive the first time or change it (address, key, agent picked by name); disconnect; pause or remove a channel; send a channel's messages to an outside webhook (add, change, remove). | S21, S22 |
| Blocks that earn their place | Hive connection card: yes, every channel runs on it and the check says what the key may do. Where Airy talks (rows with name, server, rule, on/off, last decision, remove): yes, the job of the page. Add a channel (picker by name + rule + Add) above the rows: yes, the frequent action. Webhooks, folded below: yes but secondary (an outside service, not Airy talking). | S5, S10 |
| Frequent actions and where they sit | Change a rule: one tap on the row's radios, saved at once with an Undo toast. Add a channel: picker, rule (preselected "When mentioned"), Add, on the page. Pause: the row's switch. Hive changes: a Change button on the card opens a modal. | S10, S16 |
| States per block | Connection: none (inline call to connect), checking (busy line), fine, a missing right (amber line saying which), unreachable (red). Channels: loading (skeleton), none (EmptyState), a channel gone from its server (row says so, rule still editable, remove offered), last check failed (amber line), paused (grey dot, name dimmed). Picker: no servers readable (Alert pointing at the token in Settings), every channel already added (picker says so). Webhooks: none, loading, failure. Every failed save: toast with the reason, the control returns to its saved value. | |
| Feedback per interaction | Rule radio: fills at once, toast "#chat answers when it's worth it" with Undo. Add: button busy, the row appears at the top, picker clears, toast. Pause: switch flips, toast. Remove: confirm naming the channel, row leaves, toast with Undo (re-adds). Connection save: busy, the card rechecks and shows the new facts. | S12 |
| Progress | None. | S18 |
| Text rules | Rules as people say them; the chosen rule's one-line help under the radios in the add form only. Channel and server names as Discord writes them, ids as a quiet mono line only when the name is unknown. URLs, keys and ids mono. No paragraphs in forms. | S9 |
| Reference keeper and kit component per block | Agent API webhooks (cards with a status dot), Evil Money's one-screen entry (chips with a default, verb button). Kit: PageHeader, Card, StatusDot, Badge, Button, Select (picker), RadioGroup (rules), Toggle, Modal, ConfirmDialog, Field, TextField, Collapsible, EmptyState, Skeleton, Alert, useToast. | S10, kit |
| Sizes and colour | Kit sizes; phone 44 px. Accent for the selected rule and primary buttons; amber for "cannot decide", red for unreachable or failed saves. | S11, S20 |
| Cannot be decided from here (ask the owner) | Whether a channel may have its own agent (today one agent per bot, as asked: same voice everywhere). | |

Measured before: new page.
Measured after: (end of round)
Owner reaction: (open)
