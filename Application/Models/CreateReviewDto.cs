namespace Application.Models;

public sealed record CreateReviewDto(string ReviewText, int Value, int BookingId);
