# UI brief: R2 panel shell, sign-in and Home

| Slot | Decision | Rule ids applied |
|---|---|---|
| Surface | Operator tool: the owner and his mods run Discord bots. Kit rules as written (one accent, mono for Discord ids, counts and times; sans for sentences), desktop first, phone must work. Airy's identity from `@airy/ui` (sky on navy, cloud mark). | kit, S13, S19 |
| Usual case (one sentence) | The owner opens Airy to see whether a bot is fine (where it talks and how, whether its Hive link works, what needs a look) and goes to the page that fixes it. | S21 |
| Exception paths | Sign in (password, then Discord); switch bot; add a bot (Settings, R4); sign out; install the app or take an update. | S21, S22 |
| Blocks that earn their place | Sidebar: one navigation column, yes (nine pages). Bot switcher at the top of the sidebar: yes, every page is per bot. Top bar: Update / Install pill and sign out only, yes (stack rule). Home header (bot avatar, name, on or off, id in mono): yes, says which bot. Needs a look (one Alert per problem, each with the page that fixes it), only when something is wrong: yes, it is the reason to open Home. Where Airy talks (channel name, server, rule, last decision): yes, the new feature and the main fact. Stat strip (channels, commands on, suggestions waiting, counting record), each a link: yes, one glance at what runs. No session card, no ping button, no bot list (the switcher is the bot list). | S5, S6, S10 |
| Frequent actions and where they sit | Switch bot: sidebar top. Open a page: sidebar. Fix a problem: the button inside its Alert. On phones a top bar holds the menu button that slides the same sidebar in (one navigation surface). | S15, S16 |
| States per block | Shell: signed out (to /login), loading bots (LoadingState), no bots (EmptyState pointing at Settings), bots failed (Alert with retry). Home: loading (Skeleton rows), no Hive connection (Alert), Hive unreachable or a missing right (Alert per problem), no channels (EmptyState with "Pick a channel"), a channel gone from its server (row says so), long names (ellipsis, full name in title). Sign-in: empty field (Continue disabled), wrong password (field error), rate limited, then the Discord step. | |
| Feedback per interaction | Switch bot: the page reloads its data with a busy dim, the switcher shows the new avatar, choice kept across reloads. Sign in: Continue shows busy, error under the field; Discord button navigates. Update: pill shows busy and reloads. Sign out: back to /login. | S12 |
| Progress | Sign-in has two steps: a two-segment bar. | S18 |
| Text rules | Short labels, a `//` mono line under page titles at most. Channel names as Discord writes them; ids only as a quiet mono second line. The answer rules read as people say them: "Always", "When mentioned", "When it's worth it". | S9, S19 |
| Reference keeper and kit component per block | Evil Money's layout (AppSidebar with sections, top bar with ActionPill). Kit: AppSidebar, SidebarSection, SidebarItem, BrandMark (Airy), Select (bot switcher), PageHeader, Alert, StatStrip and Stat, Card, Badge, StatusDot, EmptyState, LoadingState, Skeleton, TextField, Button, ProgressBar, ActionPill, ToastHost. | kit |
| Sizes and colour | Kit sizes, 44 px targets on phones. Accent only for the active nav item, primary buttons and links. Rule chips neutral; problems in the warn colour (amber here), failures in danger. The bot's own image colours no longer repaint the panel: Airy keeps one identity, the bot's image stays its avatar (Look, R4). | S11, S13, S20 |
| Cannot be decided from here (ask the owner) | Whether the bot's image should still recolour the panel (the old Theme page did); proposed no. | |

Measured before: the old panel is reachable nowhere; no baseline.
Measured after: (end of round)
Owner reaction: (open)
