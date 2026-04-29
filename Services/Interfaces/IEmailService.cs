namespace NotificationService.Services.Interfaces;

public interface IEmailService
{
    Task SendAsync(string toAddress, string subject, string body, CancellationToken cancellationToken = default);
}
