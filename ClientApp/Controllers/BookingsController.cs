using Application.Interfaces;
using Application.Models.Booking;
using Infrastructure.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace ClientApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController(IBookings bookings, IServiceGeneric<Bookings> bookingService, ILogger<BookingsController> logger, IMemoryCache memoryCache) : ControllerBase
    {
        [HttpGet, Route(nameof(GetBookings))]
        public async Task<IActionResult> GetBookings(int _idRoom, string _date)
        {
            logger.LogInformation("parameters query: {RoomId} - {Date}", _idRoom, _date);
            if (_idRoom <= 0 || string.IsNullOrEmpty(_date))
                return NoContent();

            return Ok(await bookings.GetSchedulesyRoom(_idRoom, _date));
        }

        [HttpGet, Route(nameof(GetAdminBookings))]
        public async Task<IActionResult> GetAdminBookings()
        {
            var result = await bookings.GetAllAsync();
            return result.Any() ? Ok(result) : NoContent();
        }

        [HttpGet, Route(nameof(GetBookingDetail))]
        public async Task<IActionResult> GetBookingDetail(int id)
        {
            if (id <= 0)
                return BadRequest();

            Bookings booking = await bookings.GetDetailAsync(id);
            return booking is null ? NoContent() : Ok(booking);
        }

        [HttpPost]
        [Route(nameof(CreateBooking))]
        public async Task<IActionResult> CreateBooking([FromBody] BookingInputDto bookingInputDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Bookings createdBooking = await bookingService.Create(bookingInputDto);
            logger.LogInformation("Created booking {BookingId}", createdBooking.Id);

            return CreatedAtAction(nameof(GetBookingDetail), new { id = createdBooking.Id }, createdBooking);
        }

        [HttpDelete, Route(nameof(DeleteBooking))]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            if (id <= 0)
                return BadRequest();

            await bookingService.Delete(id);
            return Accepted();
        }

        [HttpGet, Route(nameof(GetSchedulesByRoom))]
        public async Task<IActionResult> GetSchedulesByRoom(int idRoom, string date)
        {
            if (idRoom <= 0)
                return NoContent();

            return Ok(await bookings.GetSchedulesyRoom(idRoom, date));
        }

        [HttpGet, Route(nameof(GetBookingsByUserGuid))]
        public async Task<IActionResult> GetBookingsByUserGuid(Guid userGuid)
        {
            if (userGuid == Guid.Empty)
                return NoContent();

            return Ok(await bookings.GetBookingsByUserGuidAsync(userGuid));
        }

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
