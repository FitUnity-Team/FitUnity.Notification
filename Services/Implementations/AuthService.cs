using System.Text.Json;
using System.Text.Json.Serialization;
using NotificationService.Services.Interfaces;

namespace NotificationService.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<AuthService> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public AuthService(IHttpClientFactory httpClientFactory, ILogger<AuthService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<UserContactInfo?> GetUserContactInfoAsync(string userId, CancellationToken cancellationToken = default)
    {
        var client = _httpClientFactory.CreateClient("AuthService");

        _logger.LogInformation("Fetching contact info for user {UserId} from AuthService", userId);

        var response = await client.GetAsync($"/api/users/{userId}", cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning(
                "AuthService returned {StatusCode} for user {UserId}",
                response.StatusCode, userId
            );
            return null;
        }

        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        var userDto = JsonSerializer.Deserialize<AuthUserDto>(content, JsonOptions);

        if (userDto is null)
        {
            _logger.LogWarning("Could not deserialize AuthService response for user {UserId}", userId);
            return null;
        }

        return new UserContactInfo(
            UserId: userDto.Id,
            Email: userDto.Email,
            Phone: userDto.Phone,
            PreferredChannel: userDto.PreferredChannel ?? "Email"
        );
    }

    private sealed class AuthUserDto
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? PreferredChannel { get; set; }
    }
}
