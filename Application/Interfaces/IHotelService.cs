using Application.Models;
using Infrastructure.Models;

namespace Application.Interfaces;

public interface IHotelService : IServiceGeneric<HotelDto>, IServiceSearchByKeyword<HotelDto>
{
    Task<IEnumerable<HotelDto>> GetMyHotels(string userId);
    Task<IEnumerable<HotelDto>> GetHomeHotels(int pageNumber);

    /// <summary>Who owns a hotel (AspNetUsers.Id, may be null for legacy data). Null if the hotel doesn't exist.</summary>
    Task<HotelOwnership?> GetOwnership(int hotelId);

    /// <summary>
    /// Creates a hotel owned by <paramref name="ownerUserId"/>. Owner sign-ups start Pending
    /// (hidden from search, no payments) until a SuperAdmin approves them.
    /// </summary>
    Task<HotelDto> CreateForOwner(HotelDto entity, string ownerUserId, bool approveImmediately);

    /// <summary>SuperAdmin review queue, oldest submission first.</summary>
    Task<IReadOnlyList<PendingHotelDto>> GetPendingHotels();

    /// <summary>Approves or rejects a hotel. False if it doesn't exist.</summary>
    Task<bool> ReviewHotel(int hotelId, bool approve, string? rejectionReason, string reviewerUserId);

    /// <summary>Sends a rejected hotel back to the review queue after the owner fixed it.</summary>
    Task<bool> Resubmit(int hotelId);
}

public sealed record HotelOwnership(int HotelId, string? OwnerUserId, HotelApprovalStatus ApprovalStatus)
{
    /// <summary>Owners manage their own hotels; SuperAdmin manages all (including ownerless legacy hotels).</summary>
    public bool CanBeManagedBy(string userId, bool isSuperAdmin) =>
        isSuperAdmin || (!string.IsNullOrWhiteSpace(OwnerUserId) && OwnerUserId == userId);
}

public sealed record PendingHotelDto(
    int Id,
    string? Name,
    string? Description,
    string? Email,
    int CodeArea,
    int PhoneNumber,
    string? Street,
    string? StreetNumber,
    string? OwnerName,
    string? OwnerEmail,
    DateTime? SubmittedAtUtc);
