using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace AiryBotCode.Application.Interfaces
{
    public class ConfigurationReader : IConfigurationReader
    {
        private readonly IConfiguration _configuration;

        public ConfigurationReader(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetBotToken()
        {
            var botToken = _configuration["Bots:Token"];

            // Validate the token
            if (string.IsNullOrWhiteSpace(botToken))
                throw new InvalidOperationException("Bot token is missing. Ensure Bots:Token is set in appsettings.json.");

            return botToken;
        }
        public ulong GetBotId()
        {
            string? botIdStr = _configuration["Bots:BotId"];
            // Validate the token
            if (string.IsNullOrWhiteSpace(botIdStr))
                throw new InvalidOperationException("Bot ID is missing. Ensure Bots:BotId is set in appsettings.json.");
            // Try parse to ulong
            if (!ulong.TryParse(botIdStr, out ulong botId))
                throw new InvalidOperationException("Invalid bot ID. Ensure Bots:BotId is a valid ulong in appsettings.json.");
            return botId;
        }
        public string GetSection(string key)
        {
            string? value = _configuration[key];
            if (string.IsNullOrWhiteSpace(value))
                throw new InvalidOperationException($"Configuration section '{key}' not found in appsettings.json.");

            return value;
        }
        public ulong GetLogChannelId()
        {
            string? logChannelIdStr = _configuration["Bots:LogChannelId"];

            // Validate the token
            if (string.IsNullOrWhiteSpace(logChannelIdStr))
                throw new InvalidOperationException("Log channel ID is missing. Ensure Bots:LogChannelId is set in appsettings.json.");
            // Try parse to ulong
            if (!ulong.TryParse(logChannelIdStr, out ulong logChannelId))
                throw new InvalidOperationException("Invalid log channel ID. Ensure Bots:LogChannelId is a valid ulong in appsettings.json.");

            return logChannelId;
        }
        public string GetDatabaseConnectionString()
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("Database connection string is missing. Ensure 'ConnectionStrings:DefaultConnection' is set in appsettings.json.");
            }

            return connectionString;
        }

        public string GetOpenAIApiKey()
        {
            var apiKey = _configuration["OpenAI:ApiKey"];

            // Validate the token
            if (string.IsNullOrWhiteSpace(apiKey))
                throw new InvalidOperationException("OpenAI API key is missing. Ensure OpenAI:ApiKey is set in appsettings.json.");
            return apiKey;
        }

        public string GetOpenAIPrompt()
        {
            var prompt = _configuration["OpenAI:Prompt"];
            if (string.IsNullOrWhiteSpace(prompt))
                throw new InvalidOperationException("OpenAI prompt is missing. Ensure OpenAI:Prompt is set in appsettings.json.");
            return prompt;
        }

        public string GetBotName()
        {
            var botName = _configuration["Bots:Name"];
            if (string.IsNullOrWhiteSpace(botName))
                throw new InvalidOperationException("Bot name is missing. Ensure Bots:Name is set in appsettings.json.");
            return botName;
        }

        public bool IsBotEnabled()
        {
            var enabledStr = _configuration["Bots:Enabled"];
            if (string.IsNullOrWhiteSpace(enabledStr) || !bool.TryParse(enabledStr, out bool enabled))
                throw new InvalidOperationException("Invalid or missing bot enabled flag. Ensure Bots:Enabled is set to 'true' or 'false' in appsettings.json.");
            return enabled;
        }

        public List<ulong> GetAdminRoleIds()
        {
            var adminRoleIds = _configuration.GetSection("Bots:AdminRoleIds").Get<List<string>>();
            if (adminRoleIds == null || !adminRoleIds.Any())
                throw new InvalidOperationException("Admin role IDs are missing. Ensure Bots:AdminRoleIds is set in appsettings.json.");

            return adminRoleIds.Select(ulong.Parse).ToList();
        }

        public ulong GetEvilId()
        {
            var evilIdStr = _configuration["Bots:EvilId"];
            if (string.IsNullOrWhiteSpace(evilIdStr) || !ulong.TryParse(evilIdStr, out ulong evilId))
                throw new InvalidOperationException("Invalid or missing Evil ID. Ensure Bots:EvilId is a valid ulong in appsettings.json.");
            return evilId;
        }

        public ulong GetEvilLogChannelId()
        {
            var evilLogChannelIdStr = _configuration["Bots:EvilLogChannelId"];
            if (string.IsNullOrWhiteSpace(evilLogChannelIdStr) || !ulong.TryParse(evilLogChannelIdStr, out ulong evilLogChannelId))
                throw new InvalidOperationException("Invalid or missing Evil Log Channel ID. Ensure Bots:EvilLogChannelId is a valid ulong in appsettings.json.");
            return evilLogChannelId;
        }
    }
}
