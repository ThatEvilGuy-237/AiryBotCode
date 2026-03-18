using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace AiryBotCode.Application.Features
{
    public class EvilCommand
    {
        protected readonly DiscordSocketClient _client;
        public string Name { get; protected set; } = "none";
        public EvilCommand(IServiceProvider serviceProvider)
        {
            _client = serviceProvider.GetRequiredService<DiscordSocketClient>();
        }
        public virtual SlashCommandBuilder GetCommand()
        {
            Console.WriteLine("EvilCommand: GetCommand must be implemented by the child class!");
            throw new NotImplementedException("GetCommand must be implemented by the child class!");
        }

    }
}
