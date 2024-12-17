using AiryBotCode.Interfaces;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AiryBotCode.Events
{
    internal class MessageSendHandeler
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly IServiceProvider _serviceProvider;
        public MessageSendHandeler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public async Task HandleCommandAsync(SocketMessage arg)
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
    }
}
