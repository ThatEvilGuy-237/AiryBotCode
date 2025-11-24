using AiryBotCode.Application.Interfaces;
using AiryBotCode.Application.Interfaces.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace AiryBotCode.Infrastructure.Database.Seeders
{
    public static class BotSettingsSeeder
    {
        public static async Task Seed(IServiceProvider serviceProvider)
        {
            var configReader = serviceProvider.GetRequiredService<IConfigurationReader>();
            var botSettingRepository = serviceProvider.GetRequiredService<IBotSettingRepository>();

            var botSetting = configReader.GetBotSettings();
            if (botSetting == null) return;

            var existingSetting = await botSettingRepository.GetBotSettingAsync(botSetting.BotId);
            if (existingSetting == null)
            {
                await botSettingRepository.CreateBotSettingAsync(botSetting);
                Console.WriteLine($"- Seeded bot setting: {botSetting.BotName}");
                var result = await botSettingRepository.GetBotSettingAsync(botSetting.BotId);

                // log everything
                Console.WriteLine("BotSetting received:");
                Console.WriteLine($" BotName: {result.BotName}");
                Console.WriteLine($" OpenAIModel: {result.OpenAIModel}");
                Console.WriteLine($" OpenAIPrompt: {result.OpenAIPrompt.Substring(0, 100)}...");
                Console.WriteLine($" Enabled: {result.Enabled}");
                Console.WriteLine($" Token: {result.Token}");
                Console.WriteLine($" AdminRoleIds: {result.AdminRoleIds}");
                Console.WriteLine($" EvilId: {result.EvilId}");
                Console.WriteLine($" LogChannelId: {result.LogChannelId}");
                Console.WriteLine($" EvilLogChannelId: {result.EvilLogChannelId}");
                Console.WriteLine($" BotId: {result.BotId}");
            }
        }
    }
}