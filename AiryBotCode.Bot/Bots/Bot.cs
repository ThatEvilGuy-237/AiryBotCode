using AiryBotCode.Bot.Interfaces;
using Discord.WebSocket;
using Discord;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AiryBotCode.Infrastructure.Configuration;
using AiryBotCode.Infrastructure.DiscordEvents;
using Discord.Commands;

namespace AiryBotCode.Bot.Bots
{
    public class Bot : IBot
    {
        protected readonly DiscordSocketClient _client;
        protected readonly CommandService _commands;
        protected readonly IConfigurationReader _configuration;
        protected readonly IServiceProvider _serviceProvider;
        protected readonly MessageSendHandler _messageSendHandler;
        protected readonly SlashCommandHandler _slashCommandHandler;
        protected readonly ButtonPressHandler _buttonPressHandler;
        protected readonly FormHandler _formHandler;

        public Bot(IServiceProvider serviceProvider, IConfigurationReader configuration)
        {
            _configuration = configuration;
            _serviceProvider = serviceProvider;

            _client = _serviceProvider.GetRequiredService<DiscordSocketClient>();
            _commands = _serviceProvider.GetRequiredService<CommandService>();
            // EVENTS init
            _messageSendHandler = _serviceProvider.GetRequiredService<MessageSendHandler>();
            _slashCommandHandler = _serviceProvider.GetRequiredService<SlashCommandHandler>();
            _buttonPressHandler = _serviceProvider.GetRequiredService<ButtonPressHandler>();
            _formHandler = _serviceProvider.GetRequiredService<FormHandler>();
        }
        // Create a gloabal client service
        public static IServiceCollection CreateClientService(IServiceCollection services)
        {
            var config = new DiscordSocketConfig
            {
                GatewayIntents = GatewayIntents.Guilds | GatewayIntents.GuildMessages | GatewayIntents.MessageContent | GatewayIntents.GuildMembers
            };
            services.AddSingleton(sp =>
            {
                var config = new DiscordSocketConfig
                {
                    GatewayIntents = GatewayIntents.Guilds |
                                     GatewayIntents.GuildMessages |
                                     GatewayIntents.MessageContent |
                                     GatewayIntents.GuildMembers
                };
                return new DiscordSocketClient(config);
            });

            return services;
        }

        public virtual Task StartAsync(IServiceProvider services)
        {
            Console.WriteLine("[Virtual] Bot class, StartAsync");
            throw new NotImplementedException();
        }

        public virtual Task StopAsync()
        {
            Console.WriteLine("[Virtual] Bot class, StopAsync");
            throw new NotImplementedException();
        }
    }
}
