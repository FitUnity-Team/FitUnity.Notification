namespace NotificationService.Models.Events;

public class SessionReminderEvent
{
    public string UserId { get; set; } = string.Empty;
    public string SessionId { get; set; } = string.Empty;
    public string SessionName { get; set; } = string.Empty;
    public DateTime SessionDate { get; set; }
    public string Location { get; set; } = string.Empty;
    public int ReminderMinutesBefore { get; set; }
}
