using AiryBotCode.Interfaces;
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

namespace AiryBotCode.Events
{
    public class MessageSendHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly IServiceProvider _serviceProvider;

        public MessageSendHandler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _client = _serviceProvider.GetRequiredService<DiscordSocketClient>();
            _commands = _serviceProvider.GetRequiredService<CommandService>();
        }

        public async Task HandleCommandAsync(SocketMessage arg)
        {
            if (arg is not SocketUserMessage message || message.Author.IsBot)
            {
                return;
            }

            int position = 0;
            bool messageIsCommand = message.HasCharPrefix('!', ref position);

            await arg.Channel.SendMessageAsync("hello");
        }
    }

}
