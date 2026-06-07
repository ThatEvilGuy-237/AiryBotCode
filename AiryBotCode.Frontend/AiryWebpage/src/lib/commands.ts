// src/lib/commands.ts
//
// Registry of configurable commands shown on the Commands page.
//
// This mirrors what the backend reflection scanner emits: it reads the
// [ConfigurableCommand] attribute on each command class and the
// [LiveSetting] / [ReloadableSetting] attributes on its properties
// (see AiryBotCode.Domain/Configuration/Attributes/* and the settings
// declarations on each command in AiryBotCode.Application/Features/*) and
// projects them into CommandSetting rows. Until the API endpoint is wired
// up, this static registry stands in for that response so the UI is testable.
//
// Keep the keys / categories / live-vs-reloadable flags in sync with the
// attribute declarations on the matching command class.

import type { CommandConfig } from "./types/database";

export const mockCommandConfigs: CommandConfig[] = [
    {
        commandName: "TimeoutCommand",
        displayName: "Timeout",
        slug: "timeout",
        description: "Timeout a user and log the action for admins.",
        settings: [
            {
                key: "Description",
                value: "Timeout a user and log the action for admins",
                description: "The primary description for the command.",
                category: "General",
                uiHint: "text",
                isReloadable: true,
            },
            {
                key: "InvalidParamsMessage",
                value: "Invalid parameters provided.",
                description: "Message sent for invalid parameters.",
                category: "General",
                uiHint: "text",
                isReloadable: false,
            },
            {
                key: "SuccessMessageFormat",
                value: "🔇 **{0}** has been timed out for **{1} minutes**.\n⏰ They will be able to chat again at `{2}`.\n🛠️ Logs are being processed...",
                description: "Success message format. {0}=User, {1}=Minutes, {2}=End Time",
                category: "General",
                uiHint: "textarea",
                isReloadable: false,
            },
            {
                key: "ClearMessageChoices",
                value: JSON.stringify({ "1 hour ago": 60, "5 hours": 300, "10 hours": 600, "24 hours": 1440 }, null, 2),
                description: "Choices for the 'clear' option.",
                category: "Moderation",
                uiHint: "json",
                isReloadable: false,
            },
        ],
    },
    {
        commandName: "TalkToAiry",
        displayName: "Talk to Airy",
        slug: "talk-to-airy",
        description: "Conversational AI replies when Airy is mentioned in listening channels.",
        settings: [
            {
                key: "ListenChannelIds",
                value: "1463248705523290133,1463248955910393938,1182267222152982533,1182267222152982535,1182267222152982534,1182267779135590490,1236609199144697938",
                description: "Comma-separated channel ids Airy listens and responds in.",
                category: "Conversation",
                uiHint: "text",
                isReloadable: true,
            },
            {
                key: "SystemPrompt",
                value: "You are Airy, a helpful and playful kitsune assistant.",
                description: "System prompt prepended to every conversation.",
                category: "AI",
                uiHint: "textarea",
                isReloadable: false,
            },
            {
                key: "MaxTokens",
                value: "512",
                description: "Maximum tokens for an AI response.",
                category: "AI",
                uiHint: "number",
                isReloadable: false,
            },
        ],
    },
    {
        commandName: "GiveawayCommand",
        displayName: "Giveaway",
        slug: "start-event",
        description: "Run giveaways: collect entrants and draw random winners.",
        settings: [
            {
                key: "ScoreboardChannelId",
                value: "1182267222152982535",
                description: "Channel where the giveaway scoreboard is posted.",
                category: "Giveaway",
                uiHint: "text",
                isReloadable: true,
            },
        ],
    },
    {
        commandName: "VerifyUserAgeCommand",
        displayName: "Verify User Age",
        slug: "verif",
        description: "Age-verify a user by swapping their verified/unverified roles.",
        settings: [
            {
                key: "VerifiedRoleId",
                value: "1283099014476075151",
                description: "Role granted once a user is verified.",
                category: "Roles",
                uiHint: "text",
                isReloadable: true,
            },
            {
                key: "UnverifiedRoleId",
                value: "1283101142255144991",
                description: "Role removed once a user is verified.",
                category: "Roles",
                uiHint: "text",
                isReloadable: true,
            },
            {
                key: "LogChannelId",
                value: "1283102267129724958",
                description: "Channel where verification actions are logged.",
                category: "Logging",
                uiHint: "text",
                isReloadable: true,
            },
        ],
    },
    {
        commandName: "ReminderCommand",
        displayName: "Reminder",
        slug: "reminder",
        description: "Set a reminder that fires after a delay (e.g. 1h, 30m).",
        settings: [
            {
                key: "Description",
                value: "Set a reminder",
                description: "The primary description for the command.",
                category: "General",
                uiHint: "text",
                isReloadable: true,
            },
            {
                key: "ReminderSetTitle",
                value: "⏰ Reminder Set!",
                description: "Embed title shown when a reminder is created.",
                category: "General",
                uiHint: "text",
                isReloadable: false,
            },
        ],
    },
    {
        commandName: "UserlogsCommand",
        displayName: "User Logs",
        slug: "userlog",
        description: "Manage user moderation logs (warnings, bans, kicks, mutes).",
        settings: [
            {
                key: "Description",
                value: "Manage user logs",
                description: "The primary description for the command.",
                category: "General",
                uiHint: "text",
                isReloadable: true,
            },
            {
                key: "LogTypes",
                value: JSON.stringify(["Warning", "Ban", "Kick", "Mute"], null, 2),
                description: "Selectable log types for the 'type' option.",
                category: "Logging",
                uiHint: "json",
                isReloadable: false,
            },
            {
                key: "InvalidInputsMessage",
                value: "Invalid inputs.",
                description: "Message shown for invalid inputs.",
                category: "General",
                uiHint: "text",
                isReloadable: false,
            },
        ],
    },
    {
        commandName: "ContactUserCommand",
        displayName: "Contact User",
        slug: "contact",
        description: "Open a private channel to contact a user.",
        settings: [
            {
                key: "Description",
                value: "Create channel to contact user",
                description: "The primary description for the command.",
                category: "General",
                uiHint: "text",
                isReloadable: true,
            },
            {
                key: "ContactCategoryId",
                value: "1234577123541258280",
                description: "Category under which private contact channels are created.",
                category: "Channels",
                uiHint: "text",
                isReloadable: true,
            },
        ],
    },
    {
        commandName: "SummarizeUserCommand",
        displayName: "Summarize User",
        slug: "summarize-user",
        description: "Generate an AI summary of a user's chat history.",
        settings: [
            {
                key: "Description",
                value: "Generates an AI summary of a user's chat history.",
                description: "The primary description for the command.",
                category: "General",
                uiHint: "text",
                isReloadable: true,
            },
            {
                key: "GeneratingMessage",
                value: "Generating summary for {0}...",
                description: "Acknowledgement sent while the summary is generated. {0}=User",
                category: "AI",
                uiHint: "text",
                isReloadable: false,
            },
            {
                key: "NoSummaryMessage",
                value: "Could not generate a summary for {0}.",
                description: "Message shown when no summary could be generated. {0}=User",
                category: "AI",
                uiHint: "text",
                isReloadable: false,
            },
        ],
    },
    {
        commandName: "UntimeoutCommand",
        displayName: "Untimeout",
        slug: "untimeout",
        description: "Remove an active timeout from a user.",
        settings: [
            {
                key: "Description",
                value: "Remove a user's timeout",
                description: "The primary description for the command.",
                category: "General",
                uiHint: "text",
                isReloadable: true,
            },
            {
                key: "FailureMessage",
                value: "Something went wrong",
                description: "Message sent when the untimeout fails.",
                category: "General",
                uiHint: "text",
                isReloadable: false,
            },
        ],
    },
];
