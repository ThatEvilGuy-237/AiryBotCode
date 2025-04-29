using Discord;
using Discord.WebSocket;
using System.ComponentModel;
using System.Text;
namespace AiryBotCode.Tool.Frontend
{
    public struct UserLogData
    {
        public string UserMention { get; set; }
        public string Username { get; set; }
        public string UserId { get; set; }
        public string Type { get; set; }
        public string Reason { get; set; }
        public string Action { get; set; }
        public string Consequences { get; set; }
        public string LoggedBy { get; set; }
        public string LoggedByName { get; set; }
        public string AvatarUrl { get; set; }

        public UserLogData FillMeIn()
        {
            return new UserLogData
            {
                UserMention = UserMention,
                Username = Username,
                UserId = UserId,
                Type = Type,
                Reason = string.IsNullOrWhiteSpace(Reason) ? "[Fill in]" : Reason,
                Action = string.IsNullOrWhiteSpace(Action) ? "[Fill in]" : Action,
                Consequences = string.IsNullOrWhiteSpace(Consequences) ? "[Fill in]" : Consequences,
                LoggedBy = LoggedBy,
                LoggedByName = LoggedByName,
                AvatarUrl = AvatarUrl
            };
        }
    }
    public static class LogFrontend
    {


        public static Embed CreateLogEmbed(UserLogData logData)
        {
            logData = logData.FillMeIn();

            var descriptionBuilder = new StringBuilder()
                .AppendLine($"👤 **User:** {logData.UserMention}")
                .AppendLine($"🔹 **Username:** {logData.Username}")
                .AppendLine($"🆔 **User ID:** `{logData.UserId}`")
                .AppendLine()
                .AppendLine($"🔍 **Type:** {logData.Type}")
                .AppendLine($"📜 **Reason:** {logData.Reason}")
                .AppendLine($"⚡ **Action:** {logData.Action}")
                .AppendLine($"🚨 **Consequences:** {logData.Consequences}")
                .AppendLine()
                .AppendLine($"👮 **Logged by:** {logData.LoggedBy}");

            var embed = new EmbedBuilder()
                .WithTitle($"User Log - {logData.Username}")
                .WithDescription(descriptionBuilder.ToString())
                .WithFooter($"Logged by {logData.LoggedByName}")
                .WithThumbnailUrl(logData.AvatarUrl);

            switch (logData.Type)
            {
                case "Warning":
                    embed.WithColor(Color.Orange).WithTitle("⚠ Warning Issued");
                    break;
                case "Ban":
                    embed.WithColor(Color.DarkRed).WithTitle("⛔ User Banned");
                    break;
                case "Kick":
                    embed.WithColor(Color.Red).WithTitle("🚪 User Kicked");
                    break;
                case "Mute":
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
                UserMention = Extract("👤 **User:**"),
                Username = Extract("🔹 **Username:**"),
                UserId = Extract("🆔 **User ID:**").Trim('`'),
                Type = Extract("🔍 **Type:**"),
                Reason = Extract("📜 **Reason:**"),
                Action = Extract("⚡ **Action:**"),
                Consequences = Extract("🚨 **Consequences:**"),
                LoggedBy = Extract("👮 **Logged by:**"),
                LoggedByName = embed.Footer?.Text.Replace("Logged by ", "") ?? "",
                AvatarUrl = embed.Thumbnail?.Url ?? ""
            };
        }
    }
}