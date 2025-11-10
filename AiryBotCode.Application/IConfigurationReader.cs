
using System.Collections.Generic;

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
        string GetOpenAIPrompt();
        string GetBotName();
        bool IsBotEnabled();
        List<ulong> GetAdminRoleIds();
        ulong GetEvilId();
        ulong GetEvilLogChannelId();
    }

}