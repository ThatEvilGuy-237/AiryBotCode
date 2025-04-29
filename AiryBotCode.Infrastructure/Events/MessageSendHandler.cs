using AiryBotCode.Events;
using AiryBotCode.Events.SendMessage.MessageComands.TalkWithAiry;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace AiryBotCode.Infrastructure.Events
{
    public class MessageSendHandler: MyEventHandeler
    {
        private readonly TalkWithAiryManager _airyManager;
        public MessageSendHandler(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _airyManager = _serviceProvider.GetRequiredService<TalkWithAiryManager>();
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
