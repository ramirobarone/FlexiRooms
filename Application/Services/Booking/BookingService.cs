using Application.Exceptions;
using Application.Interfaces;
using Application.Models.Booking;
using Application.Models.Booking.Available;
using Infrastructure.Models;
using Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;

namespace Application.Services.Reserves
{
    public class BookingService(IRepository<Bookings> repositoryBookings,
        IRepository<Room> repositoryRoom,
        IRepository<TimesAvailable> repositorySchedule,
        IRepository<PaymentTransaction> paymentRepository,
        IRepository<HotelInfo> hotelInfoRepository,
        ILogger<BookingService> logger) : IBookings, IServiceGeneric<Bookings>
    {
        public async Task<IEnumerable<ScheduleDto>> GetSchedulesyRoom(int _idRoom, string _date)
        {
            logger.LogInformation("parameters idRoom:{_idRoom} and date: {_date}", _idRoom, _date);

            DateTime date = NormalizeToUtc(Convert.ToDateTime(_date));
            List<ScheduleDto> result = new();

            IEnumerable<Bookings> bookings = await repositoryBookings.GetAllByIdAsync(x => x.IdRoom == _idRoom
                && x.DateReserved == date,
                orderBy: null,
                include: z => z.Include(x => x.CheckInTime));

            int[] idsSchedulesReserved = bookings.Select(x => x.CheckInTime!.Id).ToArray();
            IEnumerable<TimesAvailable> schedules = await repositorySchedule.GetAllByIdAsync(x => !idsSchedulesReserved.Contains(x.Id));

            foreach (TimesAvailable schedule in schedules)
            {
                result.Add(schedule);
            }

            return result;
        }

        public async Task<Bookings> Create(Bookings booking)
        {
            EntityEntry<Bookings>? bookingSaved = null;

            ArgumentNullException.ThrowIfNull(booking);
            booking.DateReserved = NormalizeToUtc(booking.DateReserved);

            if (await ExistBooking(booking))
                throw new BookingException("Hay una reserva para ese dia y hora");

            Room resultRoom = await GetRoomWithCost(booking.IdRoom);
            Cost? selectedCost = resultRoom.Costs?.FirstOrDefault(cost => cost.Id == booking.CostId) ?? resultRoom.Costs?.FirstOrDefault();
            booking.Price = selectedCost?.CostPerTime ?? booking.Price;

            if (RoomIsNotNullOrEmpty(resultRoom))
                bookingSaved = await repositoryBookings.CreateAsync(booking);

            logger.LogInformation("Bookings created: {BookingId}", bookingSaved?.Entity.Id);

            return bookingSaved?.Entity ?? throw new InvalidOperationException("No se guardo la reserva");
        }

        public async Task Delete(int id) => await repositoryBookings.DeleteAsync(id);

        public async Task<IEnumerable<Bookings>> GetAllAsync()
        {
            return await repositoryBookings.GetAllByIdAsync(x => x.Id > 0,
                query => query.OrderByDescending(x => x.DateReserved),
                include: z => z.Include(x => x.CheckInTime).Include(x => x.User));
        }

        public async Task<IEnumerable<Bookings>> GetAllById(int entity)
        {
            if (entity == 0)
                throw new ArgumentNullException(nameof(entity));

            return await repositoryBookings.GetAllByIdAsync(x => x.IdRoom == entity);
        }

        public async Task<Bookings> GetById(int id) => await repositoryBookings.GetByIdAsync(x => x.Id == id);

        public async Task<IEnumerable<UserBookingDto>> GetBookingsByUserGuidAsync(Guid userGuid)
        {
            IEnumerable<Bookings> userBookings = await repositoryBookings.GetAllByIdAsync(
                booking => booking.UserGuid == userGuid,
                query => query.OrderByDescending(booking => booking.DateReserved),
                query => query.Include(booking => booking.CheckInTime));
            int[] bookingIds = userBookings.Select(booking => booking.Id).ToArray();
            int[] roomIds = userBookings.Select(booking => booking.IdRoom).Distinct().ToArray();
            IEnumerable<Room> rooms = await repositoryRoom.GetAllByIdAsync(
                room => roomIds.Contains(room.Id),
                query => query.Include(room => room.Costs).Include(room => room.Hotels));
            IEnumerable<PaymentTransaction> payments = await paymentRepository.GetAllByIdAsync(
                payment => payment.BookingId.HasValue && bookingIds.Contains(payment.BookingId.Value));
            Dictionary<int, Room> roomsById = rooms.ToDictionary(room => room.Id);
            Dictionary<int, string> paymentStatusByBookingId = payments
                .Where(payment => payment.BookingId.HasValue)
                .GroupBy(payment => payment.BookingId!.Value)
                .ToDictionary(group => group.Key, group => group.OrderByDescending(payment => payment.CreatedAtUtc).First().Status);

            int[] hotelIds = roomsById.Values.Where(room => room.Hotels is not null).Select(room => room.Hotels!.Id).Distinct().ToArray();
            IEnumerable<HotelInfo> hotelInfos = await hotelInfoRepository.GetAllByIdAsync(hotelInfo => hotelIds.Contains(hotelInfo.HotelId));
            Dictionary<int, HotelInfo> hotelInfoByHotelId = hotelInfos.ToDictionary(hotelInfo => hotelInfo.HotelId);

            return userBookings.Select(booking =>
            {
                string startTime = booking.CheckInTime?.Time ?? "00:00";
                TimeSpan parsedStartTime = TimeSpan.TryParse(startTime, out TimeSpan value) ? value : TimeSpan.Zero;
                Room? room = roomsById.GetValueOrDefault(booking.IdRoom);
                int durationHours = room?.Costs?.FirstOrDefault(cost => cost.Id == booking.CostId)?.Hour
                    ?? room?.Costs?.FirstOrDefault()?.Hour ?? 0;
                DateTime start = booking.DateReserved.Date.Add(parsedStartTime);
                DateTime end = start.AddHours(durationHours);
                HotelInfo? hotelInfo = room?.Hotels is not null ? hotelInfoByHotelId.GetValueOrDefault(room.Hotels.Id) : null;
                string? hotelWhatsAppNumber = room?.Hotels is not null
                    ? $"549{room.Hotels.CodeArea}{room.Hotels.PhoneNumber}"
                    : null;

                return new UserBookingDto(
                    booking.Id,
                    booking.IdRoom,
                    start.Date,
                    end.Date,
                    start.ToString("HH:mm"),
                    end.ToString("HH:mm"),
                    paymentStatusByBookingId.GetValueOrDefault(booking.Id, "unknown"),
                    hotelInfo?.TerminosYCondiciones,
                    hotelInfo?.InstruccionesDeUso,
                    hotelWhatsAppNumber);
            });
        }

        public async Task<Bookings> GetDetailAsync(int id)
        {
            return await repositoryBookings.GetByIdAsync(x => x.Id == id,
                z => z.Include(x => x.CheckInTime).Include(x => x.User));
        }

        public async Task Update(Bookings entity)
        {
            logger.LogInformation("Updated {Booking}", entity);
            await repositoryBookings.UpdateAsync(entity);
        }

        private async Task<Room> GetRoomWithCost(int idRoom)
            => await repositoryRoom.GetByIdAsync(x => x.Id == idRoom, y => y.Include(y => y.Costs));

        private bool RoomIsNotNullOrEmpty(Room room) => room is not null;

        private async Task<bool> ExistBooking(Bookings booking)
            => await repositoryBookings.Exist(x => x.IdRoom == booking.IdRoom
                && x.CheckInTimeId == booking.CheckInTimeId
                && x.DateReserved == booking.DateReserved);

        private static DateTime NormalizeToUtc(DateTime value)
            => value.Kind switch
            {
                DateTimeKind.Utc => value,
                DateTimeKind.Local => value.ToUniversalTime(),
                DateTimeKind.Unspecified => DateTime.SpecifyKind(value, DateTimeKind.Utc),
                _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
            };
    }
}
