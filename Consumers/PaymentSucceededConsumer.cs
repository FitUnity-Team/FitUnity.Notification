using MassTransit;
using NotificationService.Data;
using NotificationService.Models.Events;
using NotificationService.Services.Interfaces;

namespace NotificationService.Consumers;

public class PaymentSucceededConsumer : BaseNotificationConsumer, IConsumer<PaymentSucceededEvent>
{
    public PaymentSucceededConsumer(
        IAuthService authService,
        IEmailService emailService,
        ISmsService smsService,
        NotificationDbContext dbContext,
        ILogger<PaymentSucceededConsumer> logger)
        : base(authService, emailService, smsService, dbContext, logger)
    {
    }

    public async Task Consume(ConsumeContext<PaymentSucceededEvent> context)
    {
        var evt = context.Message;

        Logger.LogInformation(
            "Processing PaymentSucceededEvent for UserId={UserId}, PaymentId={PaymentId}",
            evt.UserId, evt.PaymentId
        );

        var content =
            $"Your payment of {evt.Amount:F2} {evt.Currency} has been successfully processed. " +
            $"Payment reference: {evt.PaymentId}. " +
            $"Processed on {evt.ProcessedAt:dd/MM/yyyy HH:mm} UTC.";

        await DispatchAndPersistAsync(evt.UserId, "Payment Confirmed", content, context.CancellationToken);
    }
}
