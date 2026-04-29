using NotificationService.Services.Interfaces;

namespace NotificationService.Services.Implementations;

public class SmsService : ISmsService
{
    private readonly ILogger<SmsService> _logger;
    private readonly IConfiguration _configuration;

    public SmsService(ILogger<SmsService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public async Task SendAsync(string phoneNumber, string message, CancellationToken cancellationToken = default)
    {
        var apiUrl = _configuration["Sms:ApiUrl"];
        var senderName = _configuration["Sms:SenderName"];

        _logger.LogInformation(
            "[SMS SIMULATION] Sender: {Sender} | To: {Phone} | API: {ApiUrl}",
            senderName, phoneNumber, apiUrl
        );
        _logger.LogDebug("[SMS SIMULATION] Message: {Message}", message);

        await Task.Delay(50, cancellationToken);

        _logger.LogInformation("[SMS SIMULATION] SMS successfully sent to {Phone}", phoneNumber);
    }
}
