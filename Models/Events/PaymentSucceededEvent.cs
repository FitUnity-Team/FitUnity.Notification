namespace NotificationService.Models.Events;

public class PaymentSucceededEvent
{
    public string UserId { get; set; } = string.Empty;
    public string PaymentId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "EUR";
    public DateTime ProcessedAt { get; set; }
}
