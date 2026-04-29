using MassTransit;
using NotificationService.Data;
using NotificationService.Models.Events;
using NotificationService.Services.Interfaces;

namespace NotificationService.Consumers;

public class EventBookingCancelledConsumer : BaseNotificationConsumer, IConsumer<EventBookingCancelledEvent>
{
    public EventBookingCancelledConsumer(
        IAuthService authService,
        IEmailService emailService,
        ISmsService smsService,
        NotificationDbContext dbContext,
        ILogger<EventBookingCancelledConsumer> logger)
        : base(authService, emailService, smsService, dbContext, logger)
    {
    }

    public async Task Consume(ConsumeContext<EventBookingCancelledEvent> context)
    {
        var evt = context.Message;

        Logger.LogInformation(
            "Processing EventBookingCancelledEvent for UserId={UserId}, BookingId={BookingId}",
            evt.UserId, evt.BookingId
        );

        var content =
            $"Your booking for \"{evt.EventName}\" has been cancelled. " +
            $"Reason: {evt.CancellationReason}. " +
            $"Cancelled on {evt.CancelledAt:dd/MM/yyyy HH:mm} UTC. " +
            $"Booking reference: {evt.BookingId}. " +
            $"If this was unexpected, please contact our support team.";

        await DispatchAndPersistAsync(evt.UserId, "Booking Cancelled", content, context.CancellationToken);
    }
}
