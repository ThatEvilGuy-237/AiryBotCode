using Discord.Commands;
using Discord.WebSocket;
using Discord;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using AiryBotCode.AiryBot;
using AiryBotCode.Interfaces;
using AiryBotCode.Events;

namespace AiryBotCode
{
    public class Bot : IBot
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly IConfigurationReader _configuration;
        private readonly IServiceProvider _serviceProvider;
        private readonly MessageSendHandler _messageSendHandler;

        public Bot(IConfigurationReader configuration, IServiceProvider serviceProvider)
        {
            _configuration = configuration;
            _serviceProvider = serviceProvider;

            _client = _serviceProvider.GetRequiredService<DiscordSocketClient>();
            _commands = _serviceProvider.GetRequiredService<CommandService>();
            _messageSendHandler = _serviceProvider.GetRequiredService<MessageSendHandler>();

            var config = new DiscordSocketConfig
            {
                GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent
            };

            _client = new DiscordSocketClient(config);
        }

        public async Task StartAsync(IServiceProvider services)
        {
            var discordToken = _configuration.GetBotToken();
            await _client.LoginAsync(TokenType.Bot, discordToken);
            await _client.StartAsync();

            await _commands.AddModulesAsync(Assembly.GetExecutingAssembly(), services);

            _client.MessageReceived += _messageSendHandler.HandleCommandAsync;

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
