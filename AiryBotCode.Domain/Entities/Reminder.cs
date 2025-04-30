
namespace AiryBotCode.Domain.Entities
{
    public class ReminderTask
    {
        public string Task { get; set; }
        public bool IsCompleted { get; set; }
    }
    public class Reminder
    {
        public string UserId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<ReminderTask> Tasks { get; set; }
        public DateTime ReminderTime { get; set; }
        public Reminder()
        {
            Tasks = new List<ReminderTask>();
        }
        public Reminder(string userId, string title, DateTime reminderTime)
        {
            UserId = userId;
            Title = title;
            ReminderTime = reminderTime;
            Tasks = new List<ReminderTask>();
        }
    }
}
