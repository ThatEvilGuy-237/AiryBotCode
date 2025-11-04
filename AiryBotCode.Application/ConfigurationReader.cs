using Microsoft.Extensions.Configuration;

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
            var host = _configuration["Database:Host"];
            var port = _configuration["Database:Port"];
            var name = _configuration["Database:Name"];
            var user = _configuration["Database:User"];
            var pass = _configuration["Database:Password"];

            string connectionString = $"Host={host};Port={port};Database={name};Username={user};Password={pass}";
            // Validate the token
            if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(port) || string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(pass))
                throw new InvalidOperationException("Database connection string is missing. Ensure Database:Host, Database:Port, Database:Name, Database:User, Database:Password are set in appsettings.json.");
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
    }
}
