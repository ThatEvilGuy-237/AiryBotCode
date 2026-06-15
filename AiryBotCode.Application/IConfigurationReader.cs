
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
        string GetBotName();
        string GetThemePrimaryHex();
        bool IsBotEnabled();
        List<ulong> GetAdminRoleIds();
        ulong GetEvilId();
        ulong GetEvilLogChannelId();
        // Optional: the Hive tools websocket the bot subscribes to for outbound
        // agent effects (multi-message replies). Null/absent → the consumer is off.
        string? GetHiveEffectsUrl();
    }

}