using NotificationService.Services.Interfaces;

namespace NotificationService.Services.Implementations;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;
    private readonly IConfiguration _configuration;

    public EmailService(ILogger<EmailService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public async Task SendAsync(string toAddress, string subject, string body, CancellationToken cancellationToken = default)
    {
        var smtpHost = _configuration["Smtp:Host"];
        var smtpPort = _configuration["Smtp:Port"];
        var fromAddress = _configuration["Smtp:FromAddress"];
        var fromName = _configuration["Smtp:FromName"];

        _logger.LogInformation(
            "[EMAIL SIMULATION] From: {From} ({FromName}) | To: {To} | Subject: {Subject} | SMTP: {Host}:{Port}",
            fromAddress, fromName, toAddress, subject, smtpHost, smtpPort
        );
        _logger.LogDebug("[EMAIL SIMULATION] Body: {Body}", body);

        await Task.Delay(50, cancellationToken);

        _logger.LogInformation("[EMAIL SIMULATION] Email successfully sent to {To}", toAddress);
    }
}
