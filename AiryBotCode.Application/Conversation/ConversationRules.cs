using AiryBotCode.Application.Hive;

namespace AiryBotCode.Application.Conversation
{
    public static class ConversationRules
    {
        public static bool Hears(bool listens, bool member) => listens && member;

        public static bool WaitsForCheck(string mode, bool pinged, bool member) =>
            mode == AnswerModes.Decide && !pinged && member;

        public static bool Answers(string mode, bool pinged, CheckVerdict? verdict) => mode switch
        {
            AnswerModes.Always => true,
            AnswerModes.Decide => pinged || verdict is { SaysAnswer: true },
            _ => pinged,
        };
    }
}
