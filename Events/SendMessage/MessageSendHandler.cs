using AiryBotCode.AiryBot;
using AiryBotCode.Events.SendMessage.MessageComands.TalkWithAiry;
using AiryBotCode.Interfaces;
using AiryBotCode.Registers;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AiryBotCode.Events.SendMessage
{
    public class MessageSendHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly IServiceProvider _serviceProvider;
        private readonly TalkWithAiryManager _airyManager;

        public MessageSendHandler(IServiceProvider serviceProvider, DiscordSocketClient discordClient)
        {
            _serviceProvider = serviceProvider;
            _airyManager = _serviceProvider.GetRequiredService<TalkWithAiryManager>();
            _client = discordClient;
        }
        public MessageSendHandler(IServiceProvider serviceProvider, TalkWithAiryManager airyManager, DiscordSocketClient discordSocketClient)
        {
            _serviceProvider = serviceProvider;
            _client = discordSocketClient;
            _airyManager = airyManager;


        }

        public async Task HandleCommandAsync(SocketMessage arg)
        {
            if (arg is not SocketUserMessage message || message.Author.IsBot)
                return;
            IReadOnlyCollection<SocketGuild> guilds = _client.Guilds;

            await _airyManager.HandleCommand(arg, message.Content);
        }
    }

}
