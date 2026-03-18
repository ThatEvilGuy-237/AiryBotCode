using AiryBotCode.Application.DTOs;
using AiryBotCode.Application.Interfaces;
using AiryBotCode.Application.Interfaces.Service;
using AiryBotCode.Application.Services.AIService;
using AiryBotCode.Domain.database;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
namespace AiryBotCode.Application.Comands.ConversationalInteractions
{
    public class TalkToAiry : EvilCommand
    {
        private readonly IConversationManagerService _conversationManagerService;
        private readonly IConfigurationReader _config;
        private readonly IAIService _aiService;
        private readonly IInterpreterService _interpreterService;
        private readonly IPermissionService _permissionService;
        private static readonly SemaphoreSlim _messageSavingSemaphore = new SemaphoreSlim(1, 1);
        private static readonly Dictionary<string, (string Command, ConversationContext Context, SocketMessage OriginalMessage)> _pendingCommands = new();

        public TalkToAiry(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _config = serviceProvider.GetRequiredService<IConfigurationReader>();
            _aiService = serviceProvider.GetRequiredService<IAIService>();
            _interpreterService = serviceProvider.GetRequiredService<IInterpreterService>();
            _permissionService = serviceProvider.GetRequiredService<IPermissionService>();
            _conversationManagerService = serviceProvider.GetRequiredService<IConversationManagerService>();
        }

        public async Task ProcessMessageAsync(SocketMessage message)
        {
            List<ulong> channelsId = new List<ulong> { 1182267222152982533, 1182267222152982535, 1182267222152982534, 1182267779135590490, 1236609199144697938 };
            if (!message.Content.Contains($"<@{_config.GetBotId()}>")) return;
            if (!channelsId.Contains(message.Channel.Id)) return; // Ignore bot spam channel

            var guildChannel = message.Channel as SocketGuildChannel;
            string displayName = (message.Author as SocketGuildUser)?.DisplayName ?? message.Author.Username;
            ulong botId = _config.GetBotId();

            ConversationContext context = await _conversationManagerService.GetOrCreateConversationContextAsync(
                message.Channel.Id,
                message.Author.Id,
                displayName,
                botId
            );

            // Initial user message
            var userMessage = new Message
            {
                User = context.AuthorUser,
                UserId = context.AuthorUser.Id,
                ChannelId = message.Channel.Id,
                ChannelConversation = context.ChannelConversation,
                ChannelConversationId = context.ChannelConversation.Id,
                Context = message.Content,
                CreatedAt = message.Timestamp.UtcDateTime
            };

            var conversationHistory = new List<Message> { userMessage };

            await RunAgentLoopAsync(message, context, conversationHistory);
        }

        private async Task RunAgentLoopAsync(SocketMessage originalMessage, ConversationContext context, List<Message> currentHistory)
        {
            int maxIterations = 5;
            for (int i = 0; i < maxIterations; i++)
            {
                var fullHistory = new List<Message>
                {
                    new Message
                    {
                        User = context.SystemUser,
                        Context = context.SystemPrompt + "\nTo run commands, prefix with 'RUN_COMMAND: '.",
                        CreatedAt = DateTime.UtcNow.AddMinutes(-10)
                    }
                };
                fullHistory.AddRange(context.MessageHistory);
                fullHistory.AddRange(currentHistory);

                string responseText = await _aiService.SendMessageAsync(fullHistory);

                if (responseText.StartsWith("RUN_COMMAND: "))
                {
                    var cmd = responseText.Substring("RUN_COMMAND: ".Length).Trim();
                    bool isInstall = cmd.Contains("apt") || cmd.Contains("pip install") || cmd.Contains("npm install");

                    if (isInstall && !_permissionService.IsInstallationAllowed())
                    {
                        await RequestPermissionAsync(originalMessage, cmd, context, currentHistory);
                        return; // Stop loop and wait for button interaction
                    }

                    var result = await _interpreterService.ExecuteCommandAsync(cmd);
                    var assistantMsg = new Message
                    {
                        User = context.AiUser,
                        Context = responseText,
                        CreatedAt = DateTime.UtcNow
                    };
                    var systemResultMsg = new Message
                    {
                        User = context.SystemUser,
                        Context = $"Command Result:\n{result}",
                        CreatedAt = DateTime.UtcNow
                    };

                    currentHistory.Add(assistantMsg);
                    currentHistory.Add(systemResultMsg);

                    await originalMessage.Channel.SendMessageAsync($"Executing: `{cmd}`\nResult preview: {result.Substring(0, Math.Min(result.Length, 100))}...");
                }
                else
                {
                    // Final response
                    var finalResponseMessage = new Message
                    {
                        User = context.AiUser,
                        UserId = context.AiUser.Id,
                        ChannelId = originalMessage.Channel.Id,
                        ChannelConversation = context.ChannelConversation,
                        ChannelConversationId = context.ChannelConversation.Id,
                        Context = responseText,
                        CreatedAt = DateTime.UtcNow
                    };

                    await _messageSavingSemaphore.WaitAsync();
                    try
                    {
                        // Save only the final turn for now to keep history clean, or refactor to save all
                        await _conversationManagerService.SaveMessagesAsync(context, currentHistory.First(), finalResponseMessage);
                    }
                    finally
                    {
                        _messageSavingSemaphore.Release();
                    }

                    await originalMessage.Channel.SendMessageAsync(responseText);
                    return;
                }
            }

            await originalMessage.Channel.SendMessageAsync("I've reached my maximum number of steps for this task.");
        }

        private async Task RequestPermissionAsync(SocketMessage originalMessage, string command, ConversationContext context, List<Message> history)
        {
            var permissionId = Guid.NewGuid().ToString().Substring(0, 8);
            _pendingCommands[permissionId] = (command, context, originalMessage);

            var embed = new EmbedBuilder()
                .WithTitle("Permission Requested")
                .WithDescription($"The bot wants to run: `{command}`")
                .WithColor(Color.Orange)
                .Build();

            var component = new ComponentBuilder()
                .WithButton("Approve for 30m", $"approve_install_{permissionId}", ButtonStyle.Success)
                .WithButton("Deny", $"deny_install_{permissionId}", ButtonStyle.Danger)
                .Build();

            await originalMessage.Channel.SendMessageAsync(embed: embed, components: component);
        }

        public async Task HandleInteractionAsync(SocketMessageComponent interaction)
        {
            if (interaction.User.Id != _config.GetEvilId())
            {
                await interaction.RespondAsync("You are not authorized to approve this.", ephemeral: true);
                return;
            }

            var customId = interaction.Data.CustomId;
            var permissionId = customId.Split('_').Last();

            if (customId.StartsWith("approve_install_"))
            {
                if (_pendingCommands.TryGetValue(permissionId, out var pending))
                {
                    _permissionService.GrantInstallationPermission(TimeSpan.FromMinutes(30));
                    await interaction.RespondAsync("Permission granted for 30 minutes. Resuming task...");

                    // Resume the agent loop
                    var result = await _interpreterService.ExecuteCommandAsync(pending.Command);
                    var assistantMsg = new Message { User = pending.Context.AiUser, Context = $"RUN_COMMAND: {pending.Command}", CreatedAt = DateTime.UtcNow };
                    var resultMsg = new Message { User = pending.Context.SystemUser, Context = $"Command Result:\n{result}", CreatedAt = DateTime.UtcNow };

                    var history = new List<Message> { assistantMsg, resultMsg };
                    _pendingCommands.Remove(permissionId);
                    await RunAgentLoopAsync(pending.OriginalMessage, pending.Context, history);
                }
            }
            else if (customId.StartsWith("deny_install_"))
            {
                if (_pendingCommands.TryGetValue(permissionId, out var pending))
                {
                    await interaction.RespondAsync("Permission denied.");
                    var systemMsg = new Message { User = pending.Context.SystemUser, Context = "Installation permission denied by owner.", CreatedAt = DateTime.UtcNow };
                    _pendingCommands.Remove(permissionId);
                    await RunAgentLoopAsync(pending.OriginalMessage, pending.Context, new List<Message> { systemMsg });
                }
            }
        }
    }
}
