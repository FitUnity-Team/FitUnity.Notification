namespace NotificationService.Models.Events;

public class RefundProcessedEvent
{
    public string UserId { get; set; } = string.Empty;
    public string RefundId { get; set; } = string.Empty;
    public string OriginalPaymentId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "EUR";
    public DateTime ProcessedAt { get; set; }
}
