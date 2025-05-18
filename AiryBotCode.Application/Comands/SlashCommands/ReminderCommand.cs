
using AiryBotCode.Domain.Entities;
using Discord;
using Discord.WebSocket;
using System.Text.RegularExpressions;

namespace AiryBotCode.Application.Comands.SlashCommands
{
    public class ReminderCommand : EvilCommand
    {
        List<Reminder> reminders { get; set; }
        public ReminderCommand(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            reminders = new List<Reminder>();
            Name = "reminder";
        }

        public override SlashCommandBuilder GetCommand()
        {
            return new SlashCommandBuilder()
                .WithName(Name)
                .WithDescription("Set a reminder")
                .AddOption("title", ApplicationCommandOptionType.String, "Title of the reminder", isRequired: true)
                .AddOption("description", ApplicationCommandOptionType.String, "Description of the reminder", isRequired: false)
                .AddOption("time", ApplicationCommandOptionType.String, "Time for the reminder (e.g. 1h, 30m)", isRequired: true);
        }

        public async Task<Reminder> CreateReminder(SocketSlashCommand command)
        {
            Reminder reminder = new Reminder()
            {
                UserId = command.User.Id.ToString(),
                Title = command.Data.Options.FirstOrDefault(o => o.Name == "title")?.Value?.ToString() ?? "",
                ReminderTime = DateTime.UtcNow.Add(ParseTimeSpan(command.Data.Options.FirstOrDefault(o => o.Name == "time")?.Value?.ToString() ?? "0m") ?? TimeSpan.Zero),
                Description = command.Data.Options.FirstOrDefault(o => o.Name == "description")?.Value?.ToString() ?? ""
            };

            await SendReminderEditor(reminder, command);

            return reminder;
        }

        public async Task SendReminderEditor(Reminder reminder, SocketSlashCommand command)
        {
            // Build the embed
            string taskListDisplay = reminder.Tasks.Count == 0
            ? "*No tasks assigned.*"
            : string.Join("\n", reminder.Tasks.Select((t, i) =>
             $"{(t.IsCompleted ? "✅" : "⬜")} **Task {i + 1}:** {t.Task}"));

            var embed = new EmbedBuilder()
             .WithTitle("⏰ Reminder Set!")
             .AddField("📝 Description",
                 string.IsNullOrWhiteSpace(reminder.Description)
                     ? "*No description provided.*"
                     : reminder.Description,
                 false)
             .AddField("📋 Tasks", taskListDisplay, false)
             .AddField("👤 Created by", $"<@{reminder.UserId}>", true)
             .AddField("🗓️ Reminder Time", reminder.ReminderTime.ToString("g"), true)
             .WithColor(Color.Gold)
             .WithThumbnailUrl("https://cdn-icons-png.flaticon.com/512/4893/4893024.png")
             .WithFooter(footer => footer.Text = "⏳ Reminder System")
             .WithTimestamp(DateTime.UtcNow)
             .Build();

            // Create the "Edit" button
            var component = new ComponentBuilder()
                .WithButton("Edit", customId: $"edit_reminder", style: ButtonStyle.Primary, emote: new Emoji("✏️"))
                .Build();

            // Send the embed with the button to the channel
            await command.RespondAsync(embed: embed, components: component);
        }


        private TimeSpan? ParseTimeSpan(string input)
        {
            int hours = 0, minutes = 0;

            var hourMatch = Regex.Match(input, @"(\d+)\s*h");
            if (hourMatch.Success)
                hours = int.Parse(hourMatch.Groups[1].Value);

            var minMatch = Regex.Match(input, @"(\d+)\s*m");
            if (minMatch.Success)
                minutes = int.Parse(minMatch.Groups[1].Value);

            if (hours == 0 && minutes == 0)
                return null;

            return new TimeSpan(hours, minutes, 0);
        }

    }
}
