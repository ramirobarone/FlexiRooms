using Application.Models;

namespace Application.Interfaces;

public interface IHotelImageService
{
    Task<IReadOnlyCollection<HotelPictureDto>> UploadImagesAsync(int hotelId, IReadOnlyCollection<HotelImageUploadFile> files, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<HotelPictureDto>> GetHotelImagesAsync(int hotelId, CancellationToken cancellationToken = default);
}
