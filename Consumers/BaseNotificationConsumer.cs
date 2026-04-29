using NotificationService.Data;
using NotificationService.Models.Entities;
using NotificationService.Services.Interfaces;

namespace NotificationService.Consumers;

public abstract class BaseNotificationConsumer
{
    protected readonly IAuthService AuthService;
    protected readonly IEmailService EmailService;
    protected readonly ISmsService SmsService;
    protected readonly NotificationDbContext DbContext;
    protected readonly ILogger Logger;

    protected BaseNotificationConsumer(
        IAuthService authService,
        IEmailService emailService,
        ISmsService smsService,
        NotificationDbContext dbContext,
        ILogger logger)
    {
        AuthService = authService;
        EmailService = emailService;
        SmsService = smsService;
        DbContext = dbContext;
        Logger = logger;
    }

    protected async Task DispatchAndPersistAsync(
        string userId,
        string category,
        string content,
        CancellationToken cancellationToken)
    {
        var userInfo = await AuthService.GetUserContactInfoAsync(userId, cancellationToken);

        if (userInfo is null)
        {
            Logger.LogError(
                "Cannot send notification for user {UserId}: contact info not found in AuthService",
                userId
            );
            throw new InvalidOperationException($"User contact info not found for userId={userId}");
        }

        var channel = userInfo.PreferredChannel;

        if (channel.Equals("SMS", StringComparison.OrdinalIgnoreCase))
        {
            await SmsService.SendAsync(userInfo.Phone, content, cancellationToken);
        }
        else
        {
            var subject = $"[FitUnity] {category}";
            await EmailService.SendAsync(userInfo.Email, subject, content, cancellationToken);
        }

        var notification = new Notification
        {
            UserId = userId,
            Category = category,
            Content = content,
            Channel = channel,
            Status = "Sent",
            SentAt = DateTime.UtcNow
        };

        DbContext.Notifications.Add(notification);
        await DbContext.SaveChangesAsync(cancellationToken);

        Logger.LogInformation(
            "Notification persisted: UserId={UserId}, Category={Category}, Channel={Channel}",
            userId, category, channel
        );
    }
}
