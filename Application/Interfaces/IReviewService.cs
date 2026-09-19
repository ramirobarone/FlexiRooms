using Application.Models;

namespace Application.Interfaces;

public interface IReviewService
{
    Task<ReviewDto> CreateReviewAsync(string userId, string reviewText, int value, int bookingId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<ReviewDto>> GetMyReviewsAsync(string userId, CancellationToken cancellationToken = default);
}
