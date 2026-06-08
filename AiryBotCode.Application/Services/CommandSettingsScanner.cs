using System.Reflection;
using System.Text.Json;
using AiryBotCode.Application.Features;
using AiryBotCode.Domain.Configuration.Attributes;
using AiryBotCode.Domain.database;

namespace AiryBotCode.Application.Services
{
    /// <summary>
    /// Reflects over every [ConfigurableCommand] in the application assembly and
    /// projects its [LiveSetting] / [ReloadableSetting] properties into
    /// <see cref="CommandSetting"/> rows.
    ///
    /// Attribute metadata (description / category / ui-hint / reloadable) is read
    /// without instantiating the command. The default value comes from the
    /// property initializer, which only exists on a constructed instance, so the
    /// scanner resolves the command from DI when it can; if construction fails the
    /// row is still produced with an empty default.
    /// </summary>
    public class CommandSettingsScanner
    {
        private readonly IServiceProvider _serviceProvider;

        public CommandSettingsScanner(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public List<CommandSetting> Scan(ulong botId)
        {
            var settings = new List<CommandSetting>();

            var commandTypes = typeof(EvilCommand).Assembly.GetTypes()
                .Where(t => t.GetCustomAttribute<ConfigurableCommandAttribute>() != null);

            foreach (var type in commandTypes)
            {
                var commandAttr = type.GetCustomAttribute<ConfigurableCommandAttribute>();
                if (commandAttr == null) continue;

                object? instance = TryResolve(type);

                foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    var settingAttr = prop.GetCustomAttribute<BaseSettingAttribute>(inherit: true);
                    if (settingAttr == null) continue;

                    settings.Add(new CommandSetting
                    {
                        BotId = botId,
                        CommandName = commandAttr.Name,
                        Key = prop.Name,
                        Value = ReadValue(instance, prop, settingAttr.UiHint),
                        Description = settingAttr.Description,
                        Category = settingAttr.Category,
                        UiHint = settingAttr.UiHint,
                        IsReloadable = settingAttr is ReloadableSettingAttribute,
                        LastUpdated = DateTime.UtcNow
                    });
                }
            }

            return settings;
        }

        private object? TryResolve(Type type)
        {
            try { return _serviceProvider.GetService(type); }
            catch { return null; }
        }

        private static string ReadValue(object? instance, PropertyInfo prop, string uiHint)
        {
            if (instance == null) return string.Empty;
            try
            {
                return Serialize(prop.GetValue(instance), uiHint);
            }
            catch { return string.Empty; }
        }

        private static string Serialize(object? value, string uiHint)
        {
            if (value == null) return string.Empty;
            if (value is string s) return s;
            if (value is bool b) return b ? "true" : "false";

            var type = value.GetType();
            if (type.IsPrimitive || value is decimal)
                return Convert.ToString(value, System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty;

            // Dictionaries, arrays and other complex values -> JSON (UiHint "json").
            return JsonSerializer.Serialize(value, new JsonSerializerOptions { WriteIndented = true });
        }
    }
}
