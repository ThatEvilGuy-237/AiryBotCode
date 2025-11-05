using AiryBotCode.Infrastructure.Activitys;
using AiryBotCode.Infrastructure.Configuration;
using Discord;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using AiryBotCode.Application.Interfaces;

namespace AiryBotCode.Bot.Bots
{
    public class AiryDevBot : Bot
    {
        public AiryDevBot(IConfigurationReader configuration, IServiceProvider serviceProvider) 
            : base(serviceProvider, configuration)
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
        // Assing wanted actions to the bot
        // The interfaces of the action will auto assing the action to the correct event handler
        private List<EvilAction> GetWantedActions(IServiceProvider serviceProvider)
        {
            var actions = new List<EvilAction>();
            var actionTypes = new List<Type>
            {
                typeof(UserlogsAction),
                typeof(TimeoutAction),
                typeof(UntimeOutAction),
                typeof(VerifyUserAgeAction),
                typeof(ContactUserAction),
                typeof(TalkToAiryAction),
                //typeof(ReminderAction),
            };

            foreach (var type in actionTypes)
            {
                var action = (EvilAction)serviceProvider.GetRequiredService(type);
                actions.Add(action);
                Console.WriteLine($"[INFO] Loaded action: {type.Name}");
            }

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
            _client.MessageReceived += _messageSendHandler.HandelMessageSend;
            _client.SlashCommandExecuted += _slashCommandHandler.HandleInteractionAsync;
            _client.ButtonExecuted += _buttonPressHandler.HandleButtonInteraction;
            _client.ModalSubmitted += _formHandler.HandleFormInteraction;
            _client.UserBanned += _banHandler.HandleInteractionAsync;

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
