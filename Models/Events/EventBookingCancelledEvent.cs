namespace NotificationService.Models.Events;

public class EventBookingCancelledEvent
{
    public string UserId { get; set; } = string.Empty;
    public string BookingId { get; set; } = string.Empty;
    public string EventName { get; set; } = string.Empty;
    public string CancellationReason { get; set; } = string.Empty;
    public DateTime CancelledAt { get; set; }
}
