using AiryBotCode.Application.Interfaces;
using AiryBotCode.Application.Interfaces.Repository;
using AiryBotCode.Application.Hive;
using AiryBotCode.Infrastructure.Activitys;
using Discord;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace AiryBotCode.Bot.Bots
{
    public class AiryDevBot : Bot
    {
        public AiryDevBot(IConfigurationReader configuration, IServiceProvider serviceProvider, ILogger<AiryDevBot> logger)
            : base(serviceProvider, configuration, logger)
        {
            Console.WriteLine("[INFO] Loading actions...");
            List<EvilAction> actions = GetWantedActions(serviceProvider);
            _slashCommandHandler.AssignActions(actions);
            _messageSendHandler.AssignActions(actions);
            _buttonPressHandler.AssignActions(actions);
            _formHandler.AssignActions(actions);
            _banHandler.AssignActions(actions);
            Console.WriteLine($"[INFO] {actions.Count} actions loaded and assigned.");

        }
        // Generic, DB-driven: resolve every available command (the reflection-based
        // ActionCatalog), then keep only the ones enabled for THIS bot in the DB.
        // No per-bot code or hardcoded lists.
        private List<EvilAction> GetWantedActions(IServiceProvider serviceProvider)
        {
            var botId = _configuration.GetBotId();

            // Resolve each catalog action and index it by its [ConfigurableCommand] name.
            var byCommand = new Dictionary<string, EvilAction>();
            foreach (var type in ActionCatalog.Types)
            {
                EvilAction? action;
                try { action = serviceProvider.GetService(type) as EvilAction; }
                catch { action = null; }
                if (action == null) continue;

                var name = ActionCatalog.CommandNameOf(action);
                if (string.IsNullOrEmpty(name)) continue;
                byCommand[name] = action;
            }

            // Ensure an enablement row exists for every available command (new ones
            // default to disabled), then load only the enabled ones.
            using var scope = serviceProvider.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<IBotCommandRepository>();
            repo.EnsureSeededAsync(botId, byCommand.Keys, defaultEnabled: false).GetAwaiter().GetResult();
            var enabled = repo.GetForBotAsync(botId).GetAwaiter().GetResult()
                .Where(c => c.Enabled).Select(c => c.CommandName).ToHashSet();

            var actions = byCommand.Where(kv => enabled.Contains(kv.Key)).Select(kv => kv.Value).ToList();
            Console.WriteLine($"[INFO] {actions.Count}/{byCommand.Count} commands enabled for bot {botId}.");
            return actions;
        }

        public override async Task StartAsync(IServiceProvider services)
        {
            var discordToken = _configuration.GetBotToken();
            await _client.LoginAsync(TokenType.Bot, discordToken);
            await _client.StartAsync();
            _client.Ready += async () =>
            {
                Console.WriteLine("- Bot is fully connected and ready!");
                Console.WriteLine("[Actions And Interactions] Registering...");
            };
            // await _client.SetStatusAsync(UserStatus.Online);
            await _commands.AddModulesAsync(Assembly.GetExecutingAssembly(), services);
            // Assign Event liseners
            Console.WriteLine("[INFO] Hooking up Discord events...");
            _client.Ready += _slashCommandHandler.RegisterCommandsAsync;
            // Offload handlers to the thread pool so NONE of them block the single gateway
            // task. MessageReceived forwards to the Hive and waits on the run; if it ran inline
            // it would stall the gateway, so a button press couldn't be dispatched within
            // Discord's 3s ack window → "This interaction failed". Returning immediately keeps
            // the gateway free; the interaction handlers then DeferAsync promptly.
            _client.MessageReceived      += m => { _ = Task.Run(() => _messageSendHandler.HandelMessageSend(m)); return Task.CompletedTask; };
            _client.SlashCommandExecuted += c => { _ = Task.Run(() => _slashCommandHandler.HandleInteractionAsync(c)); return Task.CompletedTask; };
            _client.ButtonExecuted       += c => { _ = Task.Run(() => _buttonPressHandler.HandleButtonInteraction(c)); return Task.CompletedTask; };
            _client.ModalSubmitted       += m => { _ = Task.Run(() => _formHandler.HandleFormInteraction(m)); return Task.CompletedTask; };
            _client.UserBanned           += (u, g) => { _ = Task.Run(() => _banHandler.HandleInteractionAsync(u, g)); return Task.CompletedTask; };

            // Hive effect passthrough (opt-in): when a tools-WS url is configured,
            // subscribe to the Hive's outbound agent effects (the `say` tool's
            // multi-message replies) and post them back to the Discord channel.
            var effectsUrl = _configuration.GetHiveEffectsUrl();
            if (!string.IsNullOrWhiteSpace(effectsUrl))
            {
                var delivery = services.GetRequiredService<IEffectDelivery>();
                var askDelivery = services.GetRequiredService<IAskDelivery>();
                var listener = new HiveEffectListener(effectsUrl, delivery, Console.WriteLine, askDelivery);
                // Bind into the singleton gateway so the button handler can send answers
                // (ask_user) back up this same socket via IHiveResponseSender.
                services.GetRequiredService<HiveEffectGateway>().Bind(listener);
                Console.WriteLine($"[INFO] Subscribing to Hive effects at {effectsUrl}");
                _ = listener.RunAsync(CancellationToken.None);   // background for the bot's lifetime
            }
        }

        public override async Task StopAsync()
        {
            if (_client != null)
            {
                await _client.LogoutAsync();
                await _client.StopAsync();
            }
        }
    }
}
