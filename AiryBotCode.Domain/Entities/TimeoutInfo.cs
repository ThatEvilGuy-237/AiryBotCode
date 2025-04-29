using Discord.WebSocket;

namespace AiryBotCode.Domain.Entities
{
    public class TimeoutInfo
    {
        public SocketGuildUser Target { get; set; }
        public object DurationOption { get; set; }
        public string Reason { get; set; }
        public int ClearMessagesTime { get; set; }

        public TimeoutInfo()
        {

        }
        public TimeoutInfo(
            SocketGuildUser target,
            object durationOption,
            string reason,
            int clearMessagesTime)
        {
            Target = target;
            DurationOption = durationOption;
            Reason = reason;
            ClearMessagesTime = clearMessagesTime;
        }
    }
}
