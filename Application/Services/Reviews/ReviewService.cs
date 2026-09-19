using Application.Interfaces;
using Application.Models;
using Infrastructure.Context;
using Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Reviews;

public class ReviewService(FlexiRoomsContext flexiRoomsContext) : IReviewService
{
    public async Task<ReviewDto> CreateReviewAsync(string userId, string reviewText, int value, int bookingId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("User id is invalid.", nameof(userId));

        if (string.IsNullOrWhiteSpace(reviewText))
            throw new ArgumentException("El texto de la reseña es obligatorio.", nameof(reviewText));

        if (value < 1 || value > 5)
            throw new ArgumentException("La valoración debe estar entre 1 y 5.", nameof(value));

        if (bookingId <= 0)
            throw new ArgumentException("La reserva indicada no es válida.", nameof(bookingId));

        ApplicationUser? user = await flexiRoomsContext.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
        if (user is null)
            throw new ArgumentException("El usuario indicado no existe.", nameof(userId));

        Bookings? booking = await flexiRoomsContext.Bookings
            .Include(x => x.Room).ThenInclude(x => x!.Hotels)
            .FirstOrDefaultAsync(x => x.Id == bookingId && x.UserGuid == user.UserGuid, cancellationToken);
        if (booking?.Room?.Hotels is null)
            throw new ArgumentException("La reserva indicada no pertenece al usuario.", nameof(bookingId));

        int hotelId = booking.Room.Hotels.Id;
        Hotel? hotel = await flexiRoomsContext.Hotels.FirstOrDefaultAsync(h => h.Id == hotelId, cancellationToken);
        if (hotel is null)
            throw new ArgumentException("El hotel indicado no existe.", nameof(bookingId));

        string createdBy = $"{user.Name} {user.LastName}".Trim();
        if (string.IsNullOrWhiteSpace(createdBy))
            createdBy = user.Email ?? user.Id;

        Review review = new()
        {
            UserId = userId,
            ReviewText = reviewText,
            Value = value,
            HotelId = hotelId,
            CreateDate = DateTime.UtcNow,
            CreateBy = createdBy
        };

        flexiRoomsContext.Reviews.Add(review);
        await flexiRoomsContext.SaveChangesAsync(cancellationToken);

        List<int> hotelReviewValues = await flexiRoomsContext.Reviews
            .AsNoTracking()
            .Where(currentReview => currentReview.HotelId == hotelId)
            .Select(currentReview => currentReview.Value)
            .ToListAsync(cancellationToken);

        decimal reviewScore = hotelReviewValues.Count == 0
            ? 0m
            : Math.Round(hotelReviewValues.Average(currentValue => (decimal)currentValue), 2, MidpointRounding.AwayFromZero);

        hotel.ReviewScore = reviewScore;
        var saved = await flexiRoomsContext.SaveChangesAsync(cancellationToken);

        return review;
    }

    public async Task<IReadOnlyCollection<ReviewDto>> GetMyReviewsAsync(string userId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("User id is invalid.", nameof(userId));

        List<Review> reviews = await flexiRoomsContext.Reviews
            .Where(review => review.UserId == userId)
            .OrderByDescending(review => review.CreateDate)
            .ToListAsync(cancellationToken);

        return reviews.Select(review => (ReviewDto)review).ToList();
    }
}
