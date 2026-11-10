using Application.Interfaces;
using Application.Models.Booking;
using ClientApp.Authorization;
using Infrastructure.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;

namespace ClientApp.Controllers
{
    /// <summary>
    /// Availability endpoints are public (checkout); everything else requires login, and
    /// admin operations require managing the booking's hotel (owner or SuperAdmin).
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController(
        IBookings bookings,
        IServiceGeneric<Bookings> bookingService,
        HotelAccessGuard hotelAccess,
        ILogger<BookingsController> logger,
        IMemoryCache memoryCache) : ControllerBase
    {
        [AllowAnonymous]
        [HttpGet, Route(nameof(GetBookings))]
        public async Task<IActionResult> GetBookings(int _idRoom, string _date)
        {
            logger.LogInformation("parameters query: {RoomId} - {Date}", _idRoom, _date);
            if (_idRoom <= 0 || string.IsNullOrEmpty(_date))
                return NoContent();

            return Ok(await bookings.GetSchedulesyRoom(_idRoom, _date));
        }

        /// <summary>Bookings of the hotels the user manages (all hotels for SuperAdmin).</summary>
        [HttpGet, Route(nameof(GetAdminBookings))]
        public async Task<IActionResult> GetAdminBookings()
        {
            IReadOnlyCollection<int>? hotelIds = await hotelAccess.ManagedHotelIdsAsync(User);
            var result = await bookings.GetAllForHotelsAsync(hotelIds);
            return result.Any() ? Ok(result) : NoContent();
        }

        /// <summary>Includes the guest's personal data: only for the hotel's managers.</summary>
        [HttpGet, Route(nameof(GetBookingDetail))]
        public async Task<IActionResult> GetBookingDetail(int id)
        {
            if (id <= 0)
                return BadRequest();

            if ((await hotelAccess.ForBookingAsync(User, id)).ToDeniedResult(this) is { } denied)
                return denied;

            Bookings booking = await bookings.GetDetailAsync(id);
            return booking is null ? NoContent() : Ok(booking);
        }

        /// <summary>Manual booking without payment: only the room's hotel managers can create one.</summary>
        [HttpPost]
        [Route(nameof(CreateBooking))]
        public async Task<IActionResult> CreateBooking([FromBody] BookingInputDto bookingInputDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if ((await hotelAccess.ForRoomAsync(User, bookingInputDto.IdRoom)).ToDeniedResult(this) is { } denied)
                return denied;

            Bookings createdBooking = await bookingService.Create(bookingInputDto);
            logger.LogInformation("Created booking {BookingId}", createdBooking.Id);

            return CreatedAtAction(nameof(GetBookingDetail), new { id = createdBooking.Id }, createdBooking);
        }

        [HttpDelete, Route(nameof(DeleteBooking))]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            if (id <= 0)
                return BadRequest();

            if ((await hotelAccess.ForBookingAsync(User, id)).ToDeniedResult(this) is { } denied)
                return denied;

            await bookingService.Delete(id);
            return Accepted();
        }

        [AllowAnonymous]
        [HttpGet, Route(nameof(GetSchedulesByRoom))]
        public async Task<IActionResult> GetSchedulesByRoom(int idRoom, string date)
        {
            if (idRoom <= 0)
                return NoContent();

            return Ok(await bookings.GetSchedulesyRoom(idRoom, date));
        }

        [HttpGet, Route(nameof(GetBookingsByUserGuid))]
        public async Task<IActionResult> GetBookingsByUserGuid()
        {
            string? userGuidClaim = User.FindFirstValue("user_guid");
            if (!Guid.TryParse(userGuidClaim, out Guid userGuid))
                return Unauthorized();

            return Ok(await bookings.GetBookingsByUserGuidAsync(userGuid));
        }

        [AllowAnonymous]
        [HttpPost, Route(nameof(CheckTemporalAvaiabilityRoom))]
        public IActionResult CheckTemporalAvaiabilityRoom(BookingInputDto bookingDto)
        {
            string createPrimaryKey = $"{bookingDto.IdRoom}{bookingDto.CheckInTimeId}{bookingDto.Date.Date}";

            if (memoryCache.TryGetValue(createPrimaryKey, out _))
                return Ok(false);

            memoryCache.Set(createPrimaryKey, bookingDto, TimeSpan.FromMinutes(5));
            return Ok(true);
        }
    }
}
