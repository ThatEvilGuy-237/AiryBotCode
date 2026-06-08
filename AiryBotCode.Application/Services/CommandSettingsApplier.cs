using System.Reflection;
using System.Text.Json;
using AiryBotCode.Application.Features;
using AiryBotCode.Application.Interfaces.Repository;
using AiryBotCode.Domain.Configuration.Attributes;

namespace AiryBotCode.Application.Services
{
    /// <summary>
    /// Inverse of <see cref="CommandSettingsScanner"/>: reads the stored
    /// <c>CommandSetting</c> values from the database and applies them onto the
    /// live command instances resolved from DI, converting the stored string back
    /// to the property's type. Read-only properties are skipped.
    ///
    /// The command instances are the same ones the action handlers hold (both
    /// resolve from the root provider), so applying here changes the running bot's
    /// behaviour. Used at startup (apply all) and on hot-reload (reloadable only).
    /// </summary>
    public class CommandSettingsApplier
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ICommandSettingsRepository _repository;

        public CommandSettingsApplier(IServiceProvider serviceProvider, ICommandSettingsRepository repository)
        {
            _serviceProvider = serviceProvider;
            _repository = repository;
        }

        /// <param name="reloadableOnly">When true, only [ReloadableSetting] properties are applied.</param>
        public async Task<int> ApplyAsync(ulong botId, bool reloadableOnly = false)
        {
            var stored = await _repository.GetAllSettingsAsync(botId);
            var byCommand = stored.GroupBy(s => s.CommandName)
                .ToDictionary(g => g.Key, g => g.ToDictionary(s => s.Key, s => s));

            var commandTypes = typeof(EvilCommand).Assembly.GetTypes()
                .Where(t => t.GetCustomAttribute<ConfigurableCommandAttribute>() != null);

            int applied = 0;
            foreach (var type in commandTypes)
            {
                var commandAttr = type.GetCustomAttribute<ConfigurableCommandAttribute>()!;
                if (!byCommand.TryGetValue(commandAttr.Name, out var values)) continue;

                object? instance = TryResolve(type);
                if (instance == null) continue;

                foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    var settingAttr = prop.GetCustomAttribute<BaseSettingAttribute>(inherit: true);
                    if (settingAttr == null || !prop.CanWrite) continue;
                    if (reloadableOnly && settingAttr is not ReloadableSettingAttribute) continue;
                    if (!values.TryGetValue(prop.Name, out var row)) continue;

                    if (TryConvert(row.Value, prop.PropertyType, out var converted))
                    {
                        try { prop.SetValue(instance, converted); applied++; }
                        catch { /* leave the default on failure */ }
                    }
                }
            }
            return applied;
        }

        private object? TryResolve(Type type)
        {
            try { return _serviceProvider.GetService(type); }
            catch { return null; }
        }

        private static bool TryConvert(string raw, Type target, out object? value)
        {
            value = null;
            try
            {
                if (target == typeof(string)) { value = raw; return true; }
                if (target == typeof(bool)) { value = bool.Parse(raw); return true; }

                var underlying = Nullable.GetUnderlyingType(target) ?? target;
                if (underlying.IsEnum) { value = Enum.Parse(underlying, raw, ignoreCase: true); return true; }
                if (underlying.IsPrimitive || underlying == typeof(decimal))
                {
                    value = Convert.ChangeType(raw, underlying, System.Globalization.CultureInfo.InvariantCulture);
                    return true;
                }

                // Arrays, dictionaries and other complex values are stored as JSON.
                value = JsonSerializer.Deserialize(raw, target);
                return value != null;
            }
            catch
            {
                return false;
            }
        }
    }
}
