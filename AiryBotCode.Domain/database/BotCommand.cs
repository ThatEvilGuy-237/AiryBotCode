namespace AiryBotCode.Domain.database
{
    // Which commands a given bot runs. One row per (BotId, CommandName); the host
    // loads only the enabled ones. CommandName matches the [ConfigurableCommand]
    // name, so it lines up 1:1 with CommandSetting.CommandName.
    public class BotCommand
    {
        public int Id { get; set; }
        public ulong BotId { get; set; }
        public string CommandName { get; set; }
        public bool Enabled { get; set; }
    }
}
