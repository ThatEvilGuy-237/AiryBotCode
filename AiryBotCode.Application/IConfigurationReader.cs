
using AiryBotCode.Domain.database;
using System.Collections.Generic;

namespace AiryBotCode.Application.Interfaces
{
    public interface IConfigurationReader
    {
        BotSetting GetBotSettings();
        string GetBotToken();
        ulong GetBotId();
        ulong GetLogChannelId();
        string GetSection(string key);
        string GetDatabaseConnectionString();
        string GetDatabaseHost();
        int GetDatabasePort();
        string GetDatabaseName();
        string GetDatabaseUser();
        string GetDatabasePassword();
        string GetOpenAIApiKey();
        string GetOpenAIModel();
        string GetOpenAIPrompt();
        string GetBotName();
        bool IsBotEnabled();
        List<ulong> GetAdminRoleIds();
        ulong GetEvilId();
        ulong GetEvilLogChannelId();
    }

}