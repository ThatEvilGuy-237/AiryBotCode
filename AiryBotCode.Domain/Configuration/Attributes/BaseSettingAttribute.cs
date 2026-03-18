namespace AiryBotCode.Domain.Configuration.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public abstract class BaseSettingAttribute : Attribute
    {
        public string Description { get; }
        public string Category { get; set; } = "General";
        public string UiHint { get; set; } = "text"; // E.g., "text", "number", "textarea", "json"

        protected BaseSettingAttribute(string description) => Description = description;
    }
}
