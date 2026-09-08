using Application.Models;

namespace Application.Interfaces;

public interface IHotelService : IServiceGeneric<HotelDto>, IServiceSearchByKeyword<HotelDto>
{
    Task<IEnumerable<HotelDto>> GetMyHotels(string userId);
}
