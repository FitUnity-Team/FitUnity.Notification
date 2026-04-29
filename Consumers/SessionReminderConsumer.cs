using MassTransit;
using NotificationService.Data;
using NotificationService.Models.Events;
using NotificationService.Services.Interfaces;

namespace NotificationService.Consumers;

public class SessionReminderConsumer : BaseNotificationConsumer, IConsumer<SessionReminderEvent>
{
    public SessionReminderConsumer(
        IAuthService authService,
        IEmailService emailService,
        ISmsService smsService,
        NotificationDbContext dbContext,
        ILogger<SessionReminderConsumer> logger)
        : base(authService, emailService, smsService, dbContext, logger)
    {
    }

    public async Task Consume(ConsumeContext<SessionReminderEvent> context)
    {
        var evt = context.Message;

        Logger.LogInformation(
            "Processing SessionReminderEvent for UserId={UserId}, SessionId={SessionId}",
            evt.UserId, evt.SessionId
        );

        var content =
            $"Reminder: Your session \"{evt.SessionName}\" starts in {evt.ReminderMinutesBefore} minutes! " +
            $"Date: {evt.SessionDate:dd/MM/yyyy HH:mm} UTC. " +
            $"Location: {evt.Location}. " +
            $"Don't forget to bring your gear. See you soon!";

        await DispatchAndPersistAsync(evt.UserId, "Session Reminder", content, context.CancellationToken);
    }
}
