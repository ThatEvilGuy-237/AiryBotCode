using AiryBotCode.Application.Hive;
using AiryBotCode.Domain.database;

namespace AiryBotCode.Application.Conversation
{
    public enum MessageStep
    {
        Quiet,
        AnswerNow,
        AskConsent,
        CheckLater,
    }

    public enum CheckStep
    {
        Skip,
        AnswerNamed,
        AskFollowUp,
        AskJoinIn,
    }

    public static class ConversationRules
    {
        public static bool Hears(bool listens, bool member) => listens && member;

        public static MessageStep OnMessage(AiryChannel channel, bool pinged, bool member, bool hasText)
        {
            if (!channel.Enabled) return MessageStep.Quiet;
            if (pinged) return member ? MessageStep.AnswerNow : MessageStep.AskConsent;
            return channel.AnswerMode switch
            {
                AnswerModes.Always => member ? MessageStep.AnswerNow : MessageStep.AskConsent,
                AnswerModes.Decide when member && hasText => MessageStep.CheckLater,
                _ => MessageStep.Quiet,
            };
        }

        public static bool QuickCheck(bool inTalk, bool named) => inTalk || named;

        public static CheckStep OnCheck(bool hasUnread, bool named, bool inTalk, bool coolingDown)
        {
            if (!hasUnread) return CheckStep.Skip;
            if (named) return CheckStep.AnswerNamed;
            if (inTalk) return CheckStep.AskFollowUp;
            return coolingDown ? CheckStep.Skip : CheckStep.AskJoinIn;
        }
    }
}
