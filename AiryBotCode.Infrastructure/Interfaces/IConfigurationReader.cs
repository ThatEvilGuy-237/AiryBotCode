
namespace AiryBotCode.Infrastructure.Configuration
{
    public interface IConfigurationReader
    {
        void LoadConfig(string? path = null);
        string GetBotToken();
        ulong GetBotId();
        ulong GetLogChannelId();
        string GetSection(string key);
        string GetDatabaseConnectionString();
        string GetOpenAIApiKey();
    }

}