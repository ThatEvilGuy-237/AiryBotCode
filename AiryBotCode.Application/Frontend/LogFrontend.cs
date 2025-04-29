using Discord;
using Discord.WebSocket;
using System.ComponentModel;
using System.Text;
namespace AiryBotCode.Tool.Frontend
{
    public enum LogType
    {
        Warning,
        Ban,
        Kick,
        Timeout,
        Other
    }
    public struct UserLogData
    {
        public string TargetMention { get; set; }
        public string TargetName { get; set; }
        public string TargetId { get; set; }
        public LogType Type { get; set; }
        public string Reason { get; set; }
        public string Action { get; set; }
        public string Consequences { get; set; }
        public string UserPing { get; set; }
        public string UserName { get; set; }
        public string TargetAvatarUrl { get; set; }

        public UserLogData Build()
        {
            return new UserLogData
            {
                TargetMention = TargetMention,
                TargetName = TargetName,
                TargetId = TargetId,
                TargetAvatarUrl = TargetAvatarUrl,
                Type = Type,
                Reason = string.IsNullOrWhiteSpace(Reason) ? "[Fill in]" : Reason,
                Action = string.IsNullOrWhiteSpace(Action) ? "[Fill in]" : Action,
                Consequences = string.IsNullOrWhiteSpace(Consequences) ? "[Fill in]" : Consequences,
                UserPing = UserPing,
                UserName = UserName
            };
        }
        public UserLogData ExtractCommandData(SocketSlashCommand command, LogType Type)
        {
            UserLogData userLog = new UserLogData
            {
                Type = Type,
                Reason = string.IsNullOrWhiteSpace(Reason) ? "[Fill in]" : Reason,
                Action = string.IsNullOrWhiteSpace(Action) ? "[Fill in]" : Action,
                Consequences = string.IsNullOrWhiteSpace(Consequences) ? "[Fill in]" : Consequences,
                UserPing = command.User.Mention,
                UserName = command.User.Username,
            };
            return userLog;
        }
        public UserLogData SetTarget(SocketGuildUser target)
        {
            UserLogData userLog = new UserLogData
            {
                TargetMention = target.Mention,
                TargetName = target.GlobalName,
                TargetId = target.Id.ToString(),
                TargetAvatarUrl = target.GetDisplayAvatarUrl(),
            };
            return userLog;
        }
        public UserLogData SetTarget(SocketUser target)
        {
            UserLogData userLog = new UserLogData
            {
                TargetMention = target.Mention,
                TargetName = target.GlobalName,
                TargetId = target.Id.ToString(),
                TargetAvatarUrl = target.GetDisplayAvatarUrl(),
            };
            return userLog;
        }
    }
    public static class LogFrontend
    {
        public static Embed CreateLogEmbed(UserLogData userLog)
        {

            var descriptionBuilder = new StringBuilder()
                .AppendLine($"👤 **User:** {userLog.TargetMention}")
                .AppendLine($"🔹 **Username:** {userLog.TargetName}")
                .AppendLine($"🆔 **User ID:** `{userLog.TargetId}`")
                .AppendLine()
                .AppendLine($"🔍 **Type:** {userLog.Type.ToString()}")
                .AppendLine($"📜 **Reason:** {userLog.Reason}")
                .AppendLine($"⚡ **Action:** {userLog.Action}")
                .AppendLine($"🚨 **Consequences:** {userLog.Consequences}")
                .AppendLine()
                .AppendLine($"👮 **Logged by:** {userLog.UserPing}");

            var embed = new EmbedBuilder()
                .WithTitle($"User Log - {userLog.TargetName}")
                .WithDescription(descriptionBuilder.ToString())
                .WithFooter($"Logged by {userLog.UserName}")
                .WithThumbnailUrl(userLog.TargetAvatarUrl);

            switch (userLog.Type)
            {
                case LogType.Warning:
                    embed.WithColor(Color.Orange).WithTitle("⚠ Warning Issued");
                    break;
                case LogType.Ban:
                    embed.WithColor(Color.DarkRed).WithTitle("⛔ User Banned");
                    break;
                case LogType.Kick:
                    embed.WithColor(Color.Red).WithTitle("🚪 User Kicked");
                    break;
                case LogType.Timeout:
                    embed.WithColor(Color.LightGrey).WithTitle("🔇 User Muted");
                    break;
                default:
                    embed.WithColor(Color.Blue);
                    break;
            }

            return embed.Build();
        }

        public static async Task EditLogEmbedAsync(SocketModal modal, UserLogData existingData, IUserMessage message)
        {
            var formValues = new Dictionary<string, string>();

            // Loop through the components (fields) in the modal
            foreach (var input in modal.Data.Components)
            {
                // Since input is of type SocketMessageComponentData, it directly has CustomId and Value
                formValues[input.CustomId] = input.Value;
            }

            // Access form values like this
            string reason = formValues.ContainsKey("reason_input") ? formValues["reason_input"] : string.Empty;
            string action = formValues.ContainsKey("action_input") ? formValues["action_input"] : string.Empty;
            string consequences = formValues.ContainsKey("consequences_input") ? formValues["consequences_input"] : string.Empty;

            // Update existingData with the new values
            existingData.Reason = reason;
            existingData.Action = action;
            existingData.Consequences = consequences;

            // Create the new embed with the updated data
            var newEmbed = CreateLogEmbed(existingData);

            // Modify the message with the updated embed
            await message.ModifyAsync(msg =>
            {
                msg.Embed = newEmbed;
            });
        }

        public static ModalBuilder CreateEditForm(UserLogData logData, string? customId = null)
        {
            var modal = new ModalBuilder()
                .WithCustomId(customId)
                .WithTitle("Edit User Log Details")
                .AddTextInput("Reason", "reason_input", placeholder: "Enter the reason...", required: true, value: logData.Reason)
                .AddTextInput("Action", "action_input", placeholder: "Enter the action taken...", required: true, value: logData.Action)
                .AddTextInput("Consequences", "consequences_input", TextInputStyle.Paragraph, placeholder: "Describe the consequences...", required: false, value: logData.Consequences);

            return modal;
        }

        public static UserLogData ExtractLogData(IEmbed embed)
        {
            string Extract(string label)
            {
                var start = embed.Description.IndexOf(label);
                if (start == -1) return "";
                start += label.Length;
                var end = embed.Description.IndexOf('\n', start);
                return embed.Description.Substring(start, (end == -1 ? embed.Description.Length : end) - start).Trim();
            }

            return new UserLogData
            {
                TargetMention = Extract("👤 **User:**"),
                TargetName = Extract("🔹 **Username:**"),
                TargetId = Extract("🆔 **User ID:**").Trim('`'),
                Type = Enum.TryParse<LogType>(Extract("🔍 **Type:**"), ignoreCase: true, out var parsed) ? parsed : LogType.Other,
                Reason = Extract("📜 **Reason:**"),
                Action = Extract("⚡ **Action:**"),
                Consequences = Extract("🚨 **Consequences:**"),
                UserPing = Extract("👮 **Logged by:**"),
                UserName = embed.Footer?.Text.Replace("Logged by ", "") ?? "",
                TargetAvatarUrl = embed.Thumbnail?.Url ?? ""
            };
        }

    }
}