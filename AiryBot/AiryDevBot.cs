using Discord.Commands;
using Discord.WebSocket;
using Discord;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using AiryBotCode.AiryBot;
using AiryBotCode.Interfaces;
using AiryBotCode.Events.SendMessage;
using AiryBotCode.Events.SlashCommands;
using AiryBotCode.Events.JoinServer;

namespace AiryBotCode
{
    public class AiryDevBot : IBot
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly IConfigurationReader _configuration;
        private readonly IServiceProvider _serviceProvider;
        private readonly MessageSendHandler _messageSendHandler;
        private readonly SlashCommandHandler _slashCommandHandler;
        private readonly JoinServerHandler _joinServerHandle;

        public AiryDevBot(IConfigurationReader configuration, IServiceProvider serviceProvider)
        {
            _configuration = configuration;
            _serviceProvider = serviceProvider;

            _client = _serviceProvider.GetRequiredService<DiscordSocketClient>();
            _commands = _serviceProvider.GetRequiredService<CommandService>();
            // EVENTS init
            _messageSendHandler = _serviceProvider.GetRequiredService<MessageSendHandler>();
            _slashCommandHandler = _serviceProvider.GetRequiredService<SlashCommandHandler>();
            _joinServerHandle = _serviceProvider.GetRequiredService<JoinServerHandler>();

            var config = new DiscordSocketConfig
            {
                GatewayIntents = GatewayIntents.Guilds | GatewayIntents.GuildMessages | GatewayIntents.MessageContent | GatewayIntents.GuildMembers
            };

            _client = new DiscordSocketClient(config);
            // add correct client
            _slashCommandHandler.AssingClient(_client);
            _messageSendHandler.AssingClient(_client);
            _joinServerHandle.AssingClient(_client);
        }

        public async Task StartAsync(IServiceProvider services)
        {
            var discordToken = _configuration.GetBotToken();
            await _client.LoginAsync(TokenType.Bot, discordToken);
            await _client.StartAsync();
            // await _client.SetStatusAsync(UserStatus.Online);
            await _commands.AddModulesAsync(Assembly.GetExecutingAssembly(), services);
            // Assign Event liseners
            _client.Ready += _slashCommandHandler.RegisterComands;
            _client.MessageReceived += _messageSendHandler.HandleCommandAsync;
            _client.SlashCommandExecuted += _slashCommandHandler.HandleInteractionAsync;
            _client.UserJoined += _joinServerHandle.OnUserJoined;
        }

        public async Task StopAsync()
        {
            if (_client != null)
            {
                await _client.LogoutAsync();
                await _client.StopAsync();
            }
        }
    }
}
