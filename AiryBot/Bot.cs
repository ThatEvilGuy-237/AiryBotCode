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
        private readonly IConfigurationReader _configuration;
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly IServiceProvider _serviceProvider;
        private MessageSendHandeler messageSendHandeler;

        public Bot(IConfigurationReader configuration, IServiceProvider serviceProvider)
        {
            _configuration = configuration;
            _serviceProvider = serviceProvider;
            DiscordSocketConfig config = new()
            {
                GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent
            };

            _client = new DiscordSocketClient(config);
            _commands = new CommandService();

            messageSendHandeler = new MessageSendHandeler(serviceProvider);
        }

        public async Task StartAsync(IServiceProvider services)
        {
            string discordToken = _configuration.GetBotToken();
            await _client.LoginAsync(TokenType.Bot, discordToken);
            await _client.StartAsync();
            await _commands.AddModulesAsync(Assembly.GetExecutingAssembly(), services);

            // EVENT HANDELERS

            _client.MessageReceived += messageSendHandeler.HandleCommandAsync;
        }
        private async Task HandleCommandAsync(SocketMessage arg)
        {
            // Ignore messages from bots
            if (arg is not SocketUserMessage message || message.Author.IsBot)
            {
                return;
            }

            // Check if the message starts with !
            int position = 0;
            bool messageIsCommand = message.HasCharPrefix('!', ref position);

            if (messageIsCommand)
            {
                // Execute the command if it exists in the ServiceCollection
                await _commands.ExecuteAsync(
                    new SocketCommandContext(_client, message),
                    position,
                    _serviceProvider);

                return;
            }
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
