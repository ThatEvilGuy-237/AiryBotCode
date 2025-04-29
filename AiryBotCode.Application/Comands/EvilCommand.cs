using Discord;

namespace AiryBotCode.Application.Comands
{
    public class EvilCommand
    {
        public string Name { get; protected set; } = "none";
        public EvilCommand()
        {
        }
        public virtual SlashCommandBuilder GetCommand()
        {
            throw new NotImplementedException("GetCommand must be implemented by the child class!");
        }

    }
}
