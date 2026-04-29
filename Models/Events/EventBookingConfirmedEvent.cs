namespace NotificationService.Models.Events;

public class EventBookingConfirmedEvent
{
    public string UserId { get; set; } = string.Empty;
    public string BookingId { get; set; } = string.Empty;
    public string EventName { get; set; } = string.Empty;
    public DateTime EventDate { get; set; }
    public string Location { get; set; } = string.Empty;
}
