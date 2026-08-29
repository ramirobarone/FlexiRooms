using Application.Exceptions;
using Application.Interfaces;
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
        ILogger<BookingService> logger) : IBookings, IServiceGeneric<Bookings>
    {
        public async Task<IEnumerable<ScheduleDto>> GetSchedulesyRoom(int _idRoom, string _date)
        {
            logger.LogInformation("parameters idRoom:{_idRoom} and date: {_date}", _idRoom, _date);

            DateTime date = DateTime.SpecifyKind(Convert.ToDateTime(_date), DateTimeKind.Utc);
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

            if (await ExistBooking(booking))
                throw new BookingException("Hay una reserva para ese dia y hora");

            Room resultRoom = await GetRoomWithCost(booking.IdRoom);
            booking.Price = resultRoom.Cost?.CostPerTime ?? booking.Price;

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

        public async Task<IEnumerable<Bookings>> GetBookingsByUserGuidAsync(Guid userGuid)
            => await repositoryBookings.GetAllByIdAsync(x => x.UserGuid == userGuid);

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
            => await repositoryRoom.GetByIdAsync(x => x.Id == idRoom, y => y.Include(y => y.Cost));

        private bool RoomIsNotNullOrEmpty(Room room) => room is not null;

        private async Task<bool> ExistBooking(Bookings booking)
            => await repositoryBookings.Exist(x => x.IdRoom == booking.IdRoom
                && x.CheckInTimeId == booking.CheckInTimeId
                && x.DateReserved == booking.DateReserved);
    }
}
