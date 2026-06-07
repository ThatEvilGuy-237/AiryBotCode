namespace AiryBotCode.Domain.Configuration.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ConfigurableCommandAttribute : Attribute
    {
        public string Name { get; }
        public ConfigurableCommandAttribute(string name) => Name = name;
    }
}
