using Application.Interfaces;
using Application.Models;
using Application.Models.Booking.Available;
using Infrastructure.Models;
using Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;

namespace Application.Services.Rooms
{
    public class ServiceRoom(IRepository<Room> repositoryRoom,
        IRepository<Bookings> repositoryBooking,
        IRepository<TimesAvailable> repositoryTimes,
        IRepository<Hotel> repositoryHotel,
        ILogger<ServiceRoom> logger) : IServiceGeneric<RoomDto>, IServiceAvailable, ITimeRoom
    {
        public async Task<RoomDto> Create(RoomDto entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            var hotel = await repositoryHotel.GetByIdAsync(x => x.Id == entity.Id);
            var room = MapRoom(entity, hotel);
            EntityEntry<Room> createdRoom = await repositoryRoom.CreateAsync(room);

            return (RoomDto)createdRoom.Entity;
        }

        public async Task Delete(int id)
        {
            await repositoryRoom.DeleteAsync(id);
        }

        public async Task<IEnumerable<RoomDto>> GetAllById(int entity)
        {
            logger.LogInformation("Method Name {GetAllById} - Parameter: {entity}", nameof(GetAllById), entity);

            IEnumerable<Room> rooms = await repositoryRoom.GetAllByIdAsync(x => x.Hotels.Id == entity, null, z => z.Include(x => x.RoomPictures).Include(x => x.Cost));
            IList<RoomDto> roomsResult = rooms.Select(room => (RoomDto)room).ToList();

            logger.LogInformation("Method Name {GetAllById} -  Parameter: {entity} - Result: {roomsResult}", nameof(GetAllById), entity, System.Text.Json.JsonSerializer.Serialize(roomsResult));

            return roomsResult;
        }

        public async Task<RoomDto> GetById(int id)
        {
            Room room = await repositoryRoom.GetByIdAsync(x => x.Id == id, z => z.Include(x => x.RoomPictures).Include(x => x.Cost).Include(x => x.Hotels));
            return room is null ? new RoomDto() : (RoomDto)room;
        }

        public async Task<bool> IsAvailable(AvailableRequestDto availableDto)
        {
            return await repositoryBooking.Exist(x => x.IdRoom == availableDto.idRoom
                && x.CheckInTimeId == availableDto.idCheckTime
                && x.DateReserved == availableDto.date);
        }

        public async Task<IEnumerable<ScheduleDto>> GetTimesAsync()
        {
            IList<ScheduleDto> times = new List<ScheduleDto>();

            var response = await repositoryTimes.GetAllByIdAsync(x => x.Id >= 0);

            foreach (TimesAvailable time in response)
            {
                times.Add(new ScheduleDto(time.Id, time.Time));
            }

            return times;
        }

        public async Task Update(RoomDto entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            var room = await repositoryRoom.GetByIdAsync(x => x.Id == entity.Id, z => z.Include(x => x.Cost).Include(x => x.RoomPictures).Include(x => x.Hotels));
            var hotel = room.Hotels;

            room.Name = entity.Name;
            room.Description = entity.Description;
            room.BedNumbers = entity.BedNumbers;
            room.AvialableNow = entity.AvialableNow;
            room.Hotels = hotel;
            room.Cost ??= new Cost();
            room.Cost.CostPerTime = entity.Cost?.CostPerHour ?? room.Cost.CostPerTime;
            room.Cost.Hour = entity.Cost?.Hour ?? room.Cost.Hour;
            room.RoomPictures = entity.RoomPictures?.Select(p => new RoomPicture { Id = p.Id, Name = p.Path }).ToList();

            await repositoryRoom.UpdateAsync(room);
        }

        private static Room MapRoom(RoomDto entity, Hotel hotel)
        {
            return new Room
            {
                Name = entity.Name,
                Description = entity.Description,
                BedNumbers = entity.BedNumbers,
                AvialableNow = entity.AvialableNow,
                Hotels = hotel,
                Cost = entity.Cost is null ? null : new Cost
                {
                    Id = entity.Cost.Id,
                    CostPerTime = entity.Cost.CostPerHour,
                    Hour = entity.Cost.Hour
                },
                RoomPictures = entity.RoomPictures?.Select(p => new RoomPicture
                {
                    Id = p.Id,
                    Name = p.Path
                }).ToList()
            };
        }
    }
}
