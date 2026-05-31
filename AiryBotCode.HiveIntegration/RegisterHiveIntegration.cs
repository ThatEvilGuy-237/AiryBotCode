using AiryBotCode.HiveIntegration.Clients;
using AiryBotCode.HiveIntegration.Contracts;
using AiryBotCode.HiveIntegration.Orchestration;
using AiryBotCode.HiveIntegration.Prompting;
using AiryBotCode.HiveIntegration.Sessions;
using AiryBotCode.HiveIntegration.Tools;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AiryBotCode.HiveIntegration;

// Single DI extension. The bot project calls .AddHiveIntegration(configuration)
// and gets the whole stack wired up — options bound from the "Hive" section,
// named HttpClients for Mind + Wraith with the configured timeout, and the
// orchestrator + collaborators registered as scoped/singleton appropriately.
public static class RegisterHiveIntegration
{
    public static IServiceCollection AddHiveIntegration(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<HiveAgentOptions>(configuration.GetSection(HiveAgentOptions.SectionName));

        // HttpClient for Mind. Timeout reads from the bound options *at request
        // time* so changes through IOptionsMonitor would be picked up if we
        // ever wire that in; for now the config snapshot at startup wins.
        services.AddHttpClient(MindAgentClient.HttpClientName, (sp, http) =>
        {
            var opts = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<HiveAgentOptions>>().Value;
            http.Timeout = TimeSpan.FromSeconds(Math.Max(30, opts.HttpTimeoutSeconds));
        });

        services.AddHttpClient(WraithToolFetcher.HttpClientName, http =>
        {
            // Tool fetch is meant to be quick; cap so a hung Wraith doesn't
            // freeze the bot.
            http.Timeout = TimeSpan.FromSeconds(10);
        });

        services.AddSingleton<WraithToolFetcher>();             // cache shared across requests
        services.AddSingleton<HivePromptComposer>();             // pure
        services.AddSingleton<IHiveSessionResolver, HiveSessionResolver>(); // stateless

        services.AddScoped<IHiveAgentClient, MindAgentClient>();
        services.AddScoped<IHiveOrchestrator, HiveOrchestrator>();

        return services;
    }
}
