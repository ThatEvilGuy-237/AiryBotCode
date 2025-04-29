
namespace AiryBotCode.Infrastructure.Configuration
{
    public interface IConfigurationReader
    {
        void LoadConfig(string? path = null);
        public string GetBotToken();
        public ulong GetLogChannelId();
    }
}