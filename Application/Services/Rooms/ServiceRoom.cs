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

            IEnumerable<Room> rooms = await repositoryRoom.GetAllByIdAsync(x => x.Hotels.Id == entity, null, z => z.Include(x => x.RoomPictures).Include(x => x.Costs));
            IList<RoomDto> roomsResult = rooms.Select(room => (RoomDto)room).ToList();

            logger.LogInformation("Method Name {GetAllById} -  Parameter: {entity} - Result: {roomsResult}", nameof(GetAllById), entity, System.Text.Json.JsonSerializer.Serialize(roomsResult));

            return roomsResult;
        }

        public async Task<RoomDto> GetById(int id)
        {
            Room room = await repositoryRoom.GetByIdAsync(x => x.Id == id, z => z.Include(x => x.RoomPictures).Include(x => x.Costs).Include(x => x.Hotels));
            return room is null ? new RoomDto() : (RoomDto)room;
        }

        public async Task<bool> IsAvailable(AvailableRequestDto availableDto)
        {
            DateTime normalizedDate = availableDto.date.Kind switch
            {
                DateTimeKind.Utc => availableDto.date,
                DateTimeKind.Local => availableDto.date.ToUniversalTime(),
                DateTimeKind.Unspecified => DateTime.SpecifyKind(availableDto.date, DateTimeKind.Utc),
                _ => DateTime.SpecifyKind(availableDto.date, DateTimeKind.Utc)
            };

            return await repositoryBooking.Exist(x => x.IdRoom == availableDto.idRoom
                && x.CheckInTimeId == availableDto.idCheckTime
                && x.DateReserved == normalizedDate);
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

            var room = await repositoryRoom.GetByIdAsync(x => x.Id == entity.Id, z => z.Include(x => x.Costs).Include(x => x.RoomPictures).Include(x => x.Hotels));
            var hotel = room.Hotels;

            room.Name = entity.Name;
            room.Description = entity.Description;
            room.BedNumbers = entity.BedNumbers;
            room.AvialableNow = entity.AvialableNow;
            room.Hotels = hotel;
            room.Costs = entity.Costs?.Select(cost => new Cost
            {
                Id = cost.Id > 0 ? cost.Id : 0,
                RoomId = entity.Id,
                CostPerTime = cost.CostPerHour,
                Hour = cost.Hour
            }).ToList() ?? new List<Cost>();
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
                Costs = entity.Costs?.Select(cost => new Cost
                {
                    CostPerTime = cost.CostPerHour,
                    Hour = cost.Hour
                }).ToList(),
                RoomPictures = entity.RoomPictures?.Select(p => new RoomPicture
                {
                    Id = p.Id,
                    Name = p.Path
                }).ToList()
            };
        }
    }
}
