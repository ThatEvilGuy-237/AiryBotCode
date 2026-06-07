using AiryBotCode.Application.Interfaces.Repository;
using AiryBotCode.Application.Services;
using AiryBotCode.Infrastructure.Activitys;
using AiryBotCode.Infrastructure.Database.Seeders;
using AiryBotCode.Infrastructure.Interfaces;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace AiryBotCode.Infrastructure.DiscordEvents
{
    public class SlashCommandHandler : EvilEventHandler
    {
        private List<EvilAction> _slashAction;
        public void AssignActions(List<EvilAction> events)
        {
            _slashAction = events.OfType<ISlashAction>().Cast<EvilAction>().ToList();
            Console.WriteLine("SlashCommandHandler");
        }

        public SlashCommandHandler(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }

        public async Task RegisterCommandsAsync()
        {
            // Project the [ConfigurableCommand] attributes into the CommandSettings
            // table now that the full DI container is available. Failures here must
            // not stop the bot from registering its slash commands.
            try
            {
                await CommandSettingsSeeder.Seed(_serviceProvider);
                // Apply the stored values onto the live commands, then watch for
                // changes / restart requests in the background.
                await ApplySettingsAsync(reloadableOnly: false);
                StartReloadWatcher();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CommandSettings] seed/apply failed: {ex.Message}");
            }

            var guilds = _client.Guilds;

            if (!guilds.Any())
            {
                Console.WriteLine("[RegisterCommands] Bot is not in any guilds.");
                return;
            }

            // More optimal but eh
            //// Remove old commands
            foreach (var guild in guilds)
            {
                await ClearServerCommands(guild);
            }

            // Register new commands
            foreach (var slashEvent in _slashAction)
            {
                await slashEvent.RegisterCommandAsync(guilds);
            }
            Console.WriteLine("- Commands Registerd!");
            Console.WriteLine("BOT IS RUNNING CORRECTLY!");
        }
        // Apply stored CommandSettings onto the live command instances.
        private async Task<int> ApplySettingsAsync(bool reloadableOnly)
        {
            using var scope = _serviceProvider.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<ICommandSettingsRepository>();
            // Resolve command instances from the ROOT provider so we mutate the same
            // long-lived instances the action handlers use.
            var applier = new CommandSettingsApplier(_serviceProvider, repo);
            return await applier.ApplyAsync(reloadableOnly);
        }

        // Background loop: hot-applies reloadable settings when they change, and
        // restarts the process when a reload is requested from the control panel
        // (the bot runs under Docker `restart: always`, so exiting restarts it).
        private void StartReloadWatcher()
        {
            _ = Task.Run(async () =>
            {
                string? startRestartSignal;
                DateTime lastSeen;
                using (var scope = _serviceProvider.CreateScope())
                {
                    var repo = scope.ServiceProvider.GetRequiredService<ICommandSettingsRepository>();
                    startRestartSignal = await repo.GetControlValueAsync("restart");
                    lastSeen = await repo.GetMaxLastUpdatedAsync();
                }

                while (true)
                {
                    await Task.Delay(TimeSpan.FromSeconds(5));
                    try
                    {
                        string? restartSignal;
                        DateTime maxUpdated;
                        using (var scope = _serviceProvider.CreateScope())
                        {
                            var repo = scope.ServiceProvider.GetRequiredService<ICommandSettingsRepository>();
                            restartSignal = await repo.GetControlValueAsync("restart");
                            maxUpdated = await repo.GetMaxLastUpdatedAsync();
                        }

                        if (restartSignal != startRestartSignal)
                        {
                            Console.WriteLine("[Reload] Restart requested from control panel — exiting for restart.");
                            Environment.Exit(0);
                        }

                        if (maxUpdated > lastSeen)
                        {
                            lastSeen = maxUpdated;
                            var n = await ApplySettingsAsync(reloadableOnly: true);
                            Console.WriteLine($"[Reload] Hot-applied {n} reloadable setting(s).");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[Reload] watcher error: {ex.Message}");
                    }
                }
            });
        }

        public async Task ClearServerCommands(SocketGuild guild)
        {
            var applicationCommands = await guild.GetApplicationCommandsAsync();

            foreach (var command in applicationCommands)
            {
                if (command.ApplicationId == _client.CurrentUser.Id) // correct way
                {
                    try
                    {
                        await command.DeleteAsync();
                        Console.WriteLine($"[Delete] Command '{command.Name}' deleted in guild '{guild.Name}'.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error deleting command '{command.Name}' in guild '{guild.Name}': {ex.Message}");
                    }
                }
            }
        }

        public async Task HandleInteractionAsync(SocketInteraction interaction)
        {
            if (interaction is not SocketSlashCommand command)
                return;

            foreach (var slashEvent in _slashAction)
            {
                if (slashEvent.Command.Name == command.Data.Name)
                {
                    if (slashEvent is ISlashAction slashEventHandler)
                    {
                        //TODO: [READ ABOUT RESPONSES TO USER]
                        //await command.DeferAsync(ephemeral: true);
                        await slashEventHandler.ExecuteSlashCommandAsync(command);
                        return;
                    }
                    return;
                }
            }

            await command.RespondAsync("Unknown command.");
        }
    }
}
