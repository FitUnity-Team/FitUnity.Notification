using MassTransit;
using NotificationService.Data;
using NotificationService.Models.Events;
using NotificationService.Services.Interfaces;

namespace NotificationService.Consumers;

public class RefundProcessedConsumer : BaseNotificationConsumer, IConsumer<RefundProcessedEvent>
{
    public RefundProcessedConsumer(
        IAuthService authService,
        IEmailService emailService,
        ISmsService smsService,
        NotificationDbContext dbContext,
        ILogger<RefundProcessedConsumer> logger)
        : base(authService, emailService, smsService, dbContext, logger)
    {
    }

    public async Task Consume(ConsumeContext<RefundProcessedEvent> context)
    {
        var evt = context.Message;

        Logger.LogInformation(
            "Processing RefundProcessedEvent for UserId={UserId}, RefundId={RefundId}, Amount={Amount}",
            evt.UserId, evt.RefundId, evt.Amount
        );

        var content =
            $"Your refund of {evt.Amount:F2} {evt.Currency} has been successfully processed. " +
            $"Refund reference: {evt.RefundId}. " +
            $"Original payment reference: {evt.OriginalPaymentId}. " +
            $"Processed on {evt.ProcessedAt:dd/MM/yyyy HH:mm} UTC. " +
            $"The funds will appear in your account within 3-5 business days.";

        await DispatchAndPersistAsync(evt.UserId, "Refund Processed", content, context.CancellationToken);
    }
}
