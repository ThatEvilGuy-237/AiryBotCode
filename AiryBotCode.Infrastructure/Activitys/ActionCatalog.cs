using System.Reflection;
using AiryBotCode.Domain.Configuration.Attributes;

namespace AiryBotCode.Infrastructure.Activitys
{
    /// <summary>
    /// The full set of available commands, discovered generically by reflection —
    /// every concrete <see cref="EvilAction"/> in the assembly. No per-bot code or
    /// hardcoded lists: a bot runs whichever of these are enabled for it in the DB.
    /// </summary>
    public static class ActionCatalog
    {
        public static IReadOnlyList<Type> Types { get; } =
            typeof(EvilAction).Assembly.GetTypes()
                .Where(t => typeof(EvilAction).IsAssignableFrom(t) && !t.IsAbstract && t != typeof(EvilAction))
                .ToList();

        /// <summary>
        /// The [ConfigurableCommand] name backing an action (matches CommandSetting /
        /// BotCommand keys), or null if the action's command isn't configurable.
        /// </summary>
        public static string? CommandNameOf(EvilAction action)
        {
            var commandType = action.Command?.GetType();
            return commandType?.GetCustomAttribute<ConfigurableCommandAttribute>()?.Name;
        }
    }
}
