using Application.Models;

namespace Application.Interfaces;

public interface IRoomImageService
{
    Task<IReadOnlyCollection<RoomPictures>> UploadImagesAsync(int roomId, IReadOnlyCollection<RoomImageUploadFile> files, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<RoomPictures>> GetRoomImagesAsync(int roomId, CancellationToken cancellationToken = default);
    Task<bool> DeleteImageAsync(int roomId, int imageId, CancellationToken cancellationToken = default);
}
