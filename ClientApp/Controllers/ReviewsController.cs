using System.Security.Claims;
using Application.Interfaces;
using Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClientApp.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ReviewsController(IReviewService reviewService, ILogger<ReviewsController> logger) : ControllerBase
{
    [HttpGet, Route(nameof(GetMyReviews))]
    public async Task<IActionResult> GetMyReviews(CancellationToken cancellationToken)
    {
        string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        IReadOnlyCollection<ReviewDto> reviews = await reviewService.GetMyReviewsAsync(userId, cancellationToken);
        return Ok(reviews);
    }

    [HttpPost]
    public async Task<IActionResult> CreateReview([FromBody] CreateReviewDto request, CancellationToken cancellationToken)
    {
        string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        try
        {
            ReviewDto created = await reviewService.CreateReviewAsync(userId, request.ReviewText, request.Value, request.BookingId, cancellationToken);
            logger.LogInformation("Created review {ReviewId} for user {UserId}", created.Id, userId);
            return Ok(created);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Invalid review creation request for user {UserId}", userId);
            return BadRequest(ex.Message);
        }
    }
}
