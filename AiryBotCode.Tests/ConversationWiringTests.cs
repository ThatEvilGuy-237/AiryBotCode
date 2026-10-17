using AiryBotCode.Application;
using AiryBotCode.Application.Conversation;
using AiryBotCode.Application.Interfaces;
using AiryBotCode.Infrastructure.Registers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace AiryBotCode.Tests
{
    public class ConversationWiringTests
    {
        [Fact]
        public void The_bot_container_builds_the_conversation_from_singletons_only()
        {
            var services = new ServiceCollection()
                .AddLogging()
                .AddSingleton<IConfiguration>(new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Database:Host"] = "localhost",
                    ["Database:Port"] = "5432",
                    ["Database:Name"] = "test",
                    ["Database:User"] = "test",
                    ["Database:Password"] = "test",
                }).Build())
                .AddScoped<IConfigurationReader, ConfigurationReader>();
            RegisterInfrastructure.RegisterServices(services);
            using var provider = services.BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true });

            var fromRoot = provider.GetRequiredService<AiryConversation>();
            using var scope = provider.CreateScope();
            Assert.Same(fromRoot, scope.ServiceProvider.GetRequiredService<AiryConversation>());
            Assert.Same(provider.GetRequiredService<ConversationSettings>(), ConversationSettings.Default);
        }
    }
}
