using Application.Interfaces;
using Application.Models;
using Infrastructure.Context;
using Infrastructure.Models;
using Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Application.Services.HotelServices
{
    public class HotelServiceQuery(FlexiRoomsContext flexiRoomsContext,
                                   ILogger<HotelServiceQuery> logger) : IHotelService
    {
        private readonly FlexiRoomsContext flexiRoomsContext = flexiRoomsContext;

        public async Task<HotelDto> Create(HotelDto entity)
        {
            flexiRoomsContext.Hotels.Add(entity);

            await flexiRoomsContext.SaveChangesAsync();

            return entity;
        }

        public async Task Delete(int id)
        {
            var hotelToDelete = await flexiRoomsContext.Hotels.FindAsync(id);
            if (hotelToDelete is null)
                throw new ArgumentNullException(nameof(hotelToDelete));
            flexiRoomsContext.Hotels.Remove(hotelToDelete);
            await flexiRoomsContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<HotelDto>> GetAllById(int id)
        {
            logger.LogInformation("MethodName: {GetAllById} - Parameter: {entity}", nameof(GetAllById), id);

            var resultQuery = await flexiRoomsContext.Hotels.Where(x => x.Id == id).Include(x => x.AddressHotel).ToListAsync();

            logger.LogInformation("MethodName: {GetAllById} - result: {entity}", nameof(GetAllById), System.Text.Json.JsonSerializer.Serialize(resultQuery));

            List<HotelDto> resultList = new();

            foreach (var result in resultQuery)
            {
                resultList.Add(result);
            }
            return resultList;
        }

        public async Task<HotelDto> GetById(int id)
        {
            var _hotel = await flexiRoomsContext.Hotels.Where(x => x.Id == id).Include(x => x.AddressHotel).FirstOrDefaultAsync();

            if (_hotel is null)
                return await Task.FromResult<HotelDto>(result: new());

            return _hotel;
        }

        public async Task<IEnumerable<HotelDto>> GetMyHotels(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("User id is required.", nameof(userId));

            List<Hotel> hotels = await flexiRoomsContext.Hotels
                .Where(x => x.IdentityNumber == userId)
                .Include(x => x.AddressHotel)
                .ToListAsync();

            List<HotelDto> hotelDtos = new();
            for (int i = 0;  i < hotels.Count; i++ )
            {
                hotelDtos.Add(hotels[i]);
            }

            return hotelDtos;
        }

        public async Task<IEnumerable<HotelDto>> SearchByKeyword(string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
                throw new ArgumentNullException(nameof(keyword));

            try
            {
                IEnumerable<Hotel> resultHotels = await flexiRoomsContext.Hotels
                    .Where(x => EF.Functions.ILike(x.MetaDescription, $"%{keyword}%"))
                    .Include(x => x.AddressHotel)
                    .Include(x => x.HotelPictures)
                    .ToListAsync();

                IList<HotelDto> hoteles = new List<HotelDto>();

                foreach (var hotel in resultHotels)
                {
                    hoteles.Add(hotel);
                }

                return hoteles;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task Update(HotelDto entity)
        {
            ArgumentNullException.ThrowIfNull(entity);

            flexiRoomsContext.Hotels.Update(entity);
            await flexiRoomsContext.SaveChangesAsync();
        }
    }
}