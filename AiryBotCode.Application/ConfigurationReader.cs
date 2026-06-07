using AiryBotCode.Domain.database;
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
        public BotSetting GetBotSettings()
        {
            return new BotSetting
            {
                BotId = GetBotId(),
                Token = GetBotToken(),
                BotName = GetBotName(),
                Enabled = IsBotEnabled(),
                OpenAIModel = GetOpenAIModel(),
                OpenAIPrompt = GetOpenAIPrompt(),
                AdminRoleIds = string.Join(",", GetAdminRoleIds()),
                EvilId = GetEvilId(),
                EvilLogChannelId = GetEvilLogChannelId(),
                LogChannelId = GetLogChannelId(),
            };
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
        public string GetDatabaseHost()
        {
            var host = _configuration["Database:Host"];
            if (string.IsNullOrWhiteSpace(host))
                throw new InvalidOperationException("Host. Ensure host is in appsettings.json.");

            return host;
        }

        public int GetDatabasePort()
        {
            var portStr = _configuration["Database:Port"];
            if (string.IsNullOrWhiteSpace(portStr) || !int.TryParse(portStr, out int port))
                throw new InvalidOperationException("Invalid or missing database port. Ensure Database:Port is a valid integer in appsettings.json.");
            return port;
        }

        public string GetDatabaseName()
        {
            var name = _configuration["Database:Name"];
            if (string.IsNullOrWhiteSpace(name))
                throw new InvalidOperationException("Database name is missing. Ensure Database:Name is set in appsettings.json.");
            return name;
        }

        public string GetDatabaseUser()
        {
            var user = _configuration["Database:User"];
            if (string.IsNullOrWhiteSpace(user))
                throw new InvalidOperationException("Database user is missing. Ensure Database:User is set in appsettings.json.");
            return user;
        }

        public string GetDatabasePassword()
        {
            var password = _configuration["Database:Password"];
            if (string.IsNullOrWhiteSpace(password))
                throw new InvalidOperationException("Database password is missing. Ensure Database:Password is set in appsettings.json.");
            return password;
        }

        public string GetDatabaseConnectionString()
        {
            var host = GetDatabaseHost();
            var port = GetDatabasePort();
            var name = GetDatabaseName();
            var user = GetDatabaseUser();
            var password = GetDatabasePassword();

            return $"Host={host};Port={port};Database={name};Username={user};Password={password}";
        }

        public string GetOpenAIApiKey()
        {
            var apiKey = _configuration["OpenAI:ApiKey"];

            // Validate the token :TODO
            if (string.IsNullOrWhiteSpace(apiKey))
                throw new InvalidOperationException("OpenAI API key is missing. Ensure OpenAI:ApiKey is set in appsettings.json.");
            return apiKey;
        }
        public string GetOpenAIModel()
        {
            var prompt = _configuration["OpenAI:Model"];
            if (string.IsNullOrWhiteSpace(prompt))
                throw new InvalidOperationException("OpenAI prompt is missing. Ensure OpenAI:Prompt is set in appsettings.json.");
            return prompt;
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
