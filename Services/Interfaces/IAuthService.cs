namespace NotificationService.Services.Interfaces;

public interface IAuthService
{
    Task<UserContactInfo?> GetUserContactInfoAsync(string userId, CancellationToken cancellationToken = default);
}

public record UserContactInfo(
    string UserId,
    string Email,
    string Phone,
    string PreferredChannel
);
