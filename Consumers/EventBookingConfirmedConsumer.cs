using MassTransit;
using NotificationService.Data;
using NotificationService.Models.Events;
using NotificationService.Services.Interfaces;

namespace NotificationService.Consumers;

public class EventBookingConfirmedConsumer : BaseNotificationConsumer, IConsumer<EventBookingConfirmedEvent>
{
    public EventBookingConfirmedConsumer(
        IAuthService authService,
        IEmailService emailService,
        ISmsService smsService,
        NotificationDbContext dbContext,
        ILogger<EventBookingConfirmedConsumer> logger)
        : base(authService, emailService, smsService, dbContext, logger)
    {
    }

    public async Task Consume(ConsumeContext<EventBookingConfirmedEvent> context)
    {
        var evt = context.Message;

        Logger.LogInformation(
            "Processing EventBookingConfirmedEvent for UserId={UserId}, BookingId={BookingId}",
            evt.UserId, evt.BookingId
        );

        var content =
            $"Your booking for \"{evt.EventName}\" has been confirmed! " +
            $"Date: {evt.EventDate:dd/MM/yyyy HH:mm} UTC. " +
            $"Location: {evt.Location}. " +
            $"Booking reference: {evt.BookingId}.";

        await DispatchAndPersistAsync(evt.UserId, "Booking Confirmed", content, context.CancellationToken);
    }
}
