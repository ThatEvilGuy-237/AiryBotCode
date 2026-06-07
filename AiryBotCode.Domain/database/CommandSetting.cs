
namespace AiryBotCode.Domain.database
{
    public class CommandSetting
    {
        public int Id { get; set; }

        public string CommandName { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }
        
        public string Description { get; set; }
        
        public string Category { get; set; }

        public string UiHint { get; set; }
        
        public bool IsReloadable { get; set; }
        
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }
}
