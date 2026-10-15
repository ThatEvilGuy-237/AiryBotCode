# UI brief: R5 Activity, Suggestions (with the public board), Databases

| Slot | Decision | Rule ids applied |
|---|---|---|
| Surface | Operator tool, Airy identity; the public board is a consumer page for community members (no mono for sentences, 44 px controls, one primary). | kit, S13, S19 |
| Usual case (one sentence) | Activity: glance at how the counting game and the level board are going. Suggestions: read what came in and approve or reject it. Databases: rarely, look at a table. Public board: a member writes one idea and sees it land. | S21 |
| Exception paths | Activity: load more of the level board. Suggestions: write one yourself, copy or renew the public link, delete. Databases: create a database. | S21 |
| Blocks that earn their place | Activity: counting per channel (count, record, boss, last counter) and the level board: yes, the two games Airy runs. Suggestions: waiting first, then the rest by state, each with the AI review when there is one; the public link card: yes, it is how members reach the board. Databases: list and a read-only table view: yes, kept last in the menu. | S5, S10 |
| Frequent actions and where they sit | Approve and Reject on each waiting suggestion (one tap, toast with Undo where the API allows). Copy the public link on the page. | S16, S12 |
| States per block | Loading (skeletons), empty (EmptyState saying what fills it), error (Alert with retry), long titles and bodies (clamped with "more"), public board: link not valid (404), too many tries (429), sent. | |
| Feedback per interaction | Approve/Reject: the card moves to its group, toast. Delete: confirm. New link: confirm (the old link stops working), toast. Public submit: button busy, the new idea appears at the top with "sent". | S12 |
| Progress | None. | S18 |
| Text rules | Plain sentences; names and counts as data in mono only where they are numbers or ids. | S9 |
| Reference keeper and kit component per block | Home's rows and stats; kit Card, StatStrip, Stat, Badge, Button, Table (level board, data view), EmptyState, Skeleton, Alert, Modal, Field, TextField, Textarea, CopyButton, ConfirmDialog. | kit |
| Sizes and colour | Kit sizes, 44 px on phones; approved ok green, rejected muted, waiting the accent. | S11, S20 |
| Cannot be decided from here (ask the owner) | Whether members may vote on suggestions (not built). | |

Measured before: old pages, not reachable.
Measured after (2026-10-15, real data on a copy of prod; screenshots in the git-ignored `docs/screenshots/private/` because they show member names, suggestion texts and table rows): Activity, Suggestions, public board, Databases list and explorer at 320, 375 and 1440: axe 0, phone probe 0 real (left: the intended clamp of long suggestion texts). Fixed on the way: the level board wider than a phone (fixed widths dropped), 9 px labels of the bare stat strip (default strip used), the public link, long titles and Airy's review text stretching grids (children may shrink), small schema select and breadcrumb links on phones, phone text fields 48 px, the kit EmptyState padding off the scale. Flows: a member sends an idea on the public board (sent line, listed first), the panel sets it to "Not now".
Owner reaction: (open)
