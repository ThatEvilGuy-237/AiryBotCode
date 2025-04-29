using Discord.WebSocket;
using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
