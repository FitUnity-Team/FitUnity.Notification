using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotificationService.Data;
using NotificationService.Models.Entities;

namespace NotificationService.Controllers;

[ApiController]
[Route("api/notifications")]
public class NotificationsController : ControllerBase
{
    private readonly NotificationDbContext _dbContext;
    private readonly ILogger<NotificationsController> _logger; 

    public NotificationsController(NotificationDbContext dbContext, ILogger<NotificationsController> logger)    
    {
        _dbContext = dbContext;
        _logger = logger;                               
    }

    [HttpGet("history/{userId}")]
    [ProducesResponseType(typeof(IEnumerable<NotificationResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetHistory(
        string userId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Fetching notification history for UserId={UserId}", userId);

        var total = await _dbContext.Notifications
            .Where(n => n.UserId == userId)
            .CountAsync(cancellationToken);

        if (total == 0)
        {
            return NotFound(new { message = $"No notifications found for user {userId}." });
        }

        var notifications = await _dbContext.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.SentAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(n => new NotificationResponse(
                n.Id,
                n.UserId,
                n.Category,
                n.Content,
                n.Channel,
                n.Status,
                n.SentAt
            ))
            .ToListAsync(cancellationToken);

        return Ok(new
        {
            total,
            page,
            pageSize,
            data = notifications
        });
    }

    [HttpGet("health")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Health() => Ok(new { status = "healthy", timestamp = DateTime.UtcNow });
}

public record NotificationResponse(
    int Id,
    string UserId,
    string Category,
    string Content,
    string Channel,
    string Status,
    DateTime SentAt
);
