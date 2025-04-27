using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AiryBotCode.Events.SendMessage.MessageComands.TalkWithAiry
{
    public enum TalkWithAiryComandList
    {
        SayHi,
    }
    public class TalkWithAiryManager
    {
        private const string AiryName = "Airy";
        private readonly Dictionary<string, Func<SocketMessage, Task>> _commandHandlers = new();

        public TalkWithAiryManager()
        {
            // Register commands
            _commandHandlers.Add(SayHi.Key, SayHi.Execute);
        }

        public async Task HandleCommand(SocketMessage arg, string message)
        {
            if (message.StartsWith(AiryName, StringComparison.OrdinalIgnoreCase))
                return;

            // Check each command in the dictionary
            foreach (var command in _commandHandlers.Keys)
            {
                if (message.Contains(command, StringComparison.OrdinalIgnoreCase))
                {
                    await _commandHandlers[command](arg);
                    return;
                }
            }
        }
    }
}
