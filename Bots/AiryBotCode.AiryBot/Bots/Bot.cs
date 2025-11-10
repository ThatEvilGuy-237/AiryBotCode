using AiryBotCode.Bot.Interfaces;
﻿using Discord.WebSocket;
﻿using Discord;
﻿using Microsoft.Extensions.DependencyInjection;
﻿using AiryBotCode.Infrastructure.Configuration;
﻿using AiryBotCode.Infrastructure.DiscordEvents;
﻿using Discord.Commands;
﻿using AiryBotCode.Application.Interfaces;
﻿using Microsoft.Extensions.Logging; // Added for logging
﻿
﻿namespace AiryBotCode.Bot.Bots
﻿{
﻿    public class Bot : IBot
﻿    {
﻿        protected readonly DiscordSocketClient _client;
﻿        protected readonly CommandService _commands;
﻿        protected readonly IConfigurationReader _configuration;
﻿        protected readonly IServiceProvider _serviceProvider;
﻿        protected readonly MessageSendHandler _messageSendHandler;
﻿        protected readonly SlashCommandHandler _slashCommandHandler;
﻿        protected readonly ButtonPressHandler _buttonPressHandler;
﻿        protected readonly FormHandler _formHandler;
﻿        protected readonly BanHandler _banHandler;
﻿        protected readonly ILogger<Bot> _logger;
﻿
﻿        public Bot(IServiceProvider serviceProvider, IConfigurationReader configuration, ILogger<Bot> logger)
﻿        {
﻿            _configuration = configuration;
﻿            _serviceProvider = serviceProvider;

﻿            _logger = logger;
﻿            _logger.LogInformation("[DISCORD SOCKETS SETUP] Registering...");

﻿            _client = _serviceProvider.GetRequiredService<DiscordSocketClient>();
﻿            _commands = _serviceProvider.GetRequiredService<CommandService>();
﻿            // EVENTS init
﻿            _messageSendHandler = _serviceProvider.GetRequiredService<MessageSendHandler>();
            _slashCommandHandler = _serviceProvider.GetRequiredService<SlashCommandHandler>();
            _buttonPressHandler = _serviceProvider.GetRequiredService<ButtonPressHandler>();
            _formHandler = _serviceProvider.GetRequiredService<FormHandler>();
            _banHandler = _serviceProvider.GetRequiredService<BanHandler>();
        }
﻿        // Create a gloabal client service
﻿        public static IServiceCollection CreateClientService(IServiceCollection services)
﻿        {
﻿            var config = new DiscordSocketConfig
﻿            {
﻿                GatewayIntents = GatewayIntents.Guilds |
﻿                                 GatewayIntents.GuildMessages |
﻿                                 GatewayIntents.MessageContent |
﻿                                 GatewayIntents.GuildMembers | GatewayIntents.GuildBans
﻿            };
﻿            services.AddSingleton(sp => new DiscordSocketClient(config));
﻿
﻿            return services;
﻿        }
﻿
﻿        public virtual Task StartAsync(IServiceProvider services)
﻿        {
﻿            _logger.LogInformation("[Virtual] Bot class, StartAsync");
﻿            throw new NotImplementedException();
﻿        }
﻿
﻿        public virtual Task StopAsync()
﻿        {
﻿            _logger.LogInformation("[Virtual] Bot class, StopAsync");
﻿            throw new NotImplementedException();
﻿        }
﻿    }
﻿}
