using AiryBotCode.Application.Conversation;

namespace AiryBotCode.Application.Hive
{
    public static class HiveTexts
    {
        public static string Say(string key, params (string Name, string Value)[] values) => Texts.Say("hive-messages", key, values);
    }
}
