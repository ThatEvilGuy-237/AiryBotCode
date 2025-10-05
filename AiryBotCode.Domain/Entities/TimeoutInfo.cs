using Discord.WebSocket;

namespace AiryBotCode.Domain.Entities
{
    public class TimeoutInfo
    {
        public SocketGuildUser Target { get; set; }
        public int Duration { get; set; }
        public string Reason { get; set; }
        public int ClearMessagesTime { get; set; }
        public int MessagesCleared { get; set; }

        public TimeoutInfo()
        {

        }
        public TimeoutInfo(
            SocketGuildUser target,
            int durationOption,
            string reason,
            int clearMessagesTime,
            int messagesCleared)
        {
            Target = target;
            Duration = durationOption;
            Reason = reason;
            ClearMessagesTime = clearMessagesTime;
            MessagesCleared = messagesCleared;
        }
    }
}
