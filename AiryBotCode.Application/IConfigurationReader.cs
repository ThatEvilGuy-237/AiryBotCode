
namespace AiryBotCode.Application.Interfaces
{
    public interface IConfigurationReader
    {
        string GetBotToken();
        ulong GetBotId();
        ulong GetLogChannelId();
        string GetSection(string key);
        string GetDatabaseConnectionString();
        string GetOpenAIApiKey();
    }

}