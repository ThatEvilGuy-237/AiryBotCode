using Discord.WebSocket;

namespace AiryBotCode.Domain.Entities
{
    public enum LogType
    {
        Warning,
        Ban,
        Kick,
        Timeout,
        Other
    }
    public class LogInfo
    {
        public LogType Type { get; set; }
        public SocketGuildUser Target { get; set; }
        /// <summary>Fallback identity when the member is no longer in the guild
        /// (banned/kicked users leave the cache — Target is null then).</summary>
        public SocketUser TargetUser { get; set; }
        public string Reason { get; set; }
        public string Action { get; set; }
        public string Consequences { get; set; }

        public LogInfo()
        {
            Type = LogType.Other;
        }

        public LogInfo(LogType type, SocketGuildUser target, string reason = "", string action = "", string consequences = "")
        {
            Type = type;
            Target = target;
            Reason = reason;
            Action = action;
            Consequences = consequences;
        }
    }
}
