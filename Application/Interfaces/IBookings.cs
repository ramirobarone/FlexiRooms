using Application.Models.Booking.Available;
using Application.Models.Booking;
using Infrastructure.Models;

namespace Application.Interfaces
{
    public interface IBookings
    {
        Task<IEnumerable<ScheduleDto>> GetSchedulesyRoom(int _idRoom, string _date);
        Task<IEnumerable<UserBookingDto>> GetBookingsByUserGuidAsync(Guid userGuid);
        Task<IEnumerable<Bookings>> GetAllAsync();

        /// <summary>Bookings of the given hotels (null = all hotels, for SuperAdmin).</summary>
        Task<IEnumerable<Bookings>> GetAllForHotelsAsync(IReadOnlyCollection<int>? hotelIds);
        Task<Bookings> GetDetailAsync(int id);
    }
}
