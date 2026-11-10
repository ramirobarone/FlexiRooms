using System.Security.Claims;
using Infrastructure.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClientApp.Authorization
{
    public enum HotelAccess
    {
        Allowed,
        NotFound,
        Forbidden
    }

    /// <summary>
    /// Single place for the "who can manage what" rule of the admin panel:
    /// a hotel (and its rooms, bookings, images and maintenance staff) can be managed
    /// by its owner (Hotel.IdentityNumber = AspNetUsers.Id) or by a SuperAdmin.
    ///
    /// Every resource is resolved to its hotel from the database, never from the request.
    /// </summary>
    public sealed class HotelAccessGuard(FlexiRoomsContext context, ILogger<HotelAccessGuard> logger)
    {
        public const string SuperAdminRole = "SuperAdmin";

        public Task<HotelAccess> ForHotelAsync(ClaimsPrincipal user, int hotelId, CancellationToken cancellationToken = default) =>
            CheckAsync(user, "hotel", hotelId,
                context.Hotels.Where(hotel => hotel.Id == hotelId)
                    .Select(hotel => new OwnerRow(hotel.IdentityNumber)),
                cancellationToken);

        public Task<HotelAccess> ForRoomAsync(ClaimsPrincipal user, int roomId, CancellationToken cancellationToken = default) =>
            CheckAsync(user, "room", roomId,
                context.Rooms.Where(room => room.Id == roomId)
                    .Select(room => new OwnerRow(room.Hotels != null ? room.Hotels.IdentityNumber : null)),
                cancellationToken);

        public Task<HotelAccess> ForBookingAsync(ClaimsPrincipal user, int bookingId, CancellationToken cancellationToken = default) =>
            CheckAsync(user, "booking", bookingId,
                context.Bookings.Where(booking => booking.Id == bookingId)
                    .Select(booking => new OwnerRow(
                        booking.Room != null && booking.Room.Hotels != null ? booking.Room.Hotels.IdentityNumber : null)),
                cancellationToken);

        public Task<HotelAccess> ForMaintenanceAsync(ClaimsPrincipal user, int maintenanceId, CancellationToken cancellationToken = default) =>
            CheckAsync(user, "maintenance", maintenanceId,
                context.HotelMaintenances.Where(maintenance => maintenance.Id == maintenanceId)
                    .Select(maintenance => new OwnerRow(maintenance.Hotel != null ? maintenance.Hotel.IdentityNumber : null)),
                cancellationToken);

        /// <summary>Hotels the user can manage; null means "all" (SuperAdmin).</summary>
        public async Task<IReadOnlyCollection<int>?> ManagedHotelIdsAsync(ClaimsPrincipal user, CancellationToken cancellationToken = default)
        {
            if (user.IsInRole(SuperAdminRole))
            {
                return null;
            }

            string? userId = UserId(user);
            if (userId is null)
            {
                return [];
            }

            return await context.Hotels
                .Where(hotel => hotel.IdentityNumber == userId)
                .Select(hotel => hotel.Id)
                .ToListAsync(cancellationToken);
        }

        private async Task<HotelAccess> CheckAsync(
            ClaimsPrincipal user, string resource, int id, IQueryable<OwnerRow> query, CancellationToken cancellationToken)
        {
            if (id <= 0)
            {
                return HotelAccess.NotFound;
            }

            OwnerRow? row = await query.AsNoTracking().FirstOrDefaultAsync(cancellationToken);
            if (row is null)
            {
                return HotelAccess.NotFound;
            }

            if (user.IsInRole(SuperAdminRole))
            {
                return HotelAccess.Allowed;
            }

            string? userId = UserId(user);
            if (userId is not null && !string.IsNullOrWhiteSpace(row.OwnerUserId) && row.OwnerUserId == userId)
            {
                return HotelAccess.Allowed;
            }

            logger.LogWarning("User {UserId} denied access to {Resource} {ResourceId}.", userId ?? "(anonymous)", resource, id);
            return HotelAccess.Forbidden;
        }

        private static string? UserId(ClaimsPrincipal user)
        {
            string? userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            return string.IsNullOrWhiteSpace(userId) ? null : userId;
        }

        private sealed record OwnerRow(string? OwnerUserId);
    }

    public static class HotelAccessExtensions
    {
        /// <summary>Null when allowed; otherwise the response to return (404 / 403).</summary>
        public static IActionResult? ToDeniedResult(this HotelAccess access, ControllerBase controller) => access switch
        {
            HotelAccess.Allowed => null,
            HotelAccess.NotFound => controller.NotFound(),
            _ => controller.Forbid()
        };
    }
}
