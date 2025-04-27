using dotenv.net;
using DotNetEnv;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace AiryBotCode.Infrastructure.Configuration
{
    public class ConfigurationReader : IConfigurationReader
    {
        private readonly IConfiguration _configuration;
        public static bool loaded = false;
        public ConfigurationReader(IConfiguration configuration)
        {
            _configuration = configuration;
            LoadConfig();
        }

        public void LoadConfig(string? path = null)
        {
            if (!string.IsNullOrWhiteSpace(path))
            {
                DotEnv.Load(options: new DotEnvOptions(envFilePaths: [path]));
            }
            else
            {
                DotEnv.Load();
            }
            loaded = true;
        }

        public string GetBotToken()
        {
            LoadConfig();

            var botToken = Environment.GetEnvironmentVariable("DISCORD_AIRYBOT_TOKEN");

            // Validate the token
            if (string.IsNullOrWhiteSpace(botToken))
                throw new InvalidOperationException("Bot token is missing. Ensure DISCORD_BOT_TOKEN is set.");

            return botToken;
        }
        public string GetSection(string key)
        {
            string? value = _configuration[key];
            if (string.IsNullOrWhiteSpace(value))
                throw new InvalidOperationException($"Configuration section '{key}' not found.");

            return value;
        }
    }
}
