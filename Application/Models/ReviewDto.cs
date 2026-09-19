using Infrastructure.Models;

namespace Application.Models;

public class ReviewDto(int Id, string ReviewText, int Value, DateTime CreateDate, string CreateBy, DateTime? UpdateDate, string? UpdateBy)
{
    public int Id { get; } = Id;
    public string ReviewText { get; } = ReviewText;
    public int Value { get; } = Value;
    public DateTime CreateDate { get; } = CreateDate;
    public string CreateBy { get; } = CreateBy;
    public DateTime? UpdateDate { get; } = UpdateDate;
    public string? UpdateBy { get; } = UpdateBy;

    public static implicit operator ReviewDto(Review review)
    {
        return new ReviewDto(review.Id, review.ReviewText, review.Value, review.CreateDate, review.CreateBy, review.UpdateDate, review.UpdateBy);
    }
}
