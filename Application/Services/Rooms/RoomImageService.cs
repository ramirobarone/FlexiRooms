using System.Globalization;
using System.Text.RegularExpressions;
using Application.Interfaces;
using Application.Models;
using Infrastructure.Context;
using Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Services.Rooms;

public class RoomImageService(FlexiRoomsContext flexiRoomsContext,
                              ILogger<RoomImageService> logger) : IRoomImageService
{
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".webp"
    };

    private const int MaxFileSizeBytes = 5 * 1024 * 1024;

    public async Task<IReadOnlyCollection<RoomPictures>> UploadImagesAsync(int roomId, IReadOnlyCollection<RoomImageUploadFile> files, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Starting upload of {Count} image(s) for room {RoomId}", files?.Count ?? 0, roomId);

        if (roomId <= 0)
        {
            logger.LogWarning("Invalid room id provided for image upload: {RoomId}", roomId);
            throw new ArgumentException("Room id is invalid.", nameof(roomId));
        }

        ArgumentNullException.ThrowIfNull(files);

        if (files.Count == 0)
        {
            logger.LogWarning("No files provided in image upload for room {RoomId}", roomId);
            throw new ArgumentException("At least one image is required.", nameof(files));
        }

        Room? room = await flexiRoomsContext.Rooms
            .Include(x => x.RoomPictures)
            .FirstOrDefaultAsync(x => x.Id == roomId, cancellationToken);

        if (room is null)
        {
            logger.LogWarning("Room with id {RoomId} was not found for image upload", roomId);
            throw new InvalidOperationException("Room not found.");
        }

        string normalizedRoomName = NormalizeRoomName(room.Name, room.Id);
        string imagesDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "rooms", roomId.ToString(CultureInfo.InvariantCulture));
        Directory.CreateDirectory(imagesDirectory);
        logger.LogInformation("Target directory for room {RoomId} images: {Directory}", roomId, imagesDirectory);

        int currentIndex = GetCurrentIndex(room.RoomPictures, normalizedRoomName);

        room.RoomPictures ??= new List<RoomPicture>();
        List<RoomPicture> uploadedImages = new(files.Count);

        foreach (RoomImageUploadFile file in files)
        {
            ValidateFile(file);

            currentIndex++;
            string extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            string newFileName = $"{normalizedRoomName}-{currentIndex}{extension}";
            string physicalPath = Path.Combine(imagesDirectory, newFileName);

            while (File.Exists(physicalPath))
            {
                currentIndex++;
                newFileName = $"{normalizedRoomName}-{currentIndex}{extension}";
                physicalPath = Path.Combine(imagesDirectory, newFileName);
            }

            logger.LogInformation("Writing image {FileName} as {NewFileName} to {PhysicalPath}", file.FileName, newFileName, physicalPath);
            await File.WriteAllBytesAsync(physicalPath, file.Content, cancellationToken);

            string relativePath = $"/rooms/{roomId}/{newFileName}";
            RoomPicture roomPicture = new()
            {
                Name = relativePath
            };

            room.RoomPictures.Add(roomPicture);
            uploadedImages.Add(roomPicture);
        }

        await flexiRoomsContext.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Successfully saved {Count} image record(s) in DB for room {RoomId}", uploadedImages.Count, roomId);

        return uploadedImages.Select(x => new RoomPictures { Id = x.Id, Path = x.Name }).ToList();
    }

    public async Task<IReadOnlyCollection<RoomPictures>> GetRoomImagesAsync(int roomId, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Retrieving images for room {RoomId}", roomId);

        if (roomId <= 0)
        {
            logger.LogWarning("Invalid room id provided in GetRoomImagesAsync: {RoomId}", roomId);
            throw new ArgumentException("Room id is invalid.", nameof(roomId));
        }

        Room? room = await flexiRoomsContext.Rooms
            .Include(x => x.RoomPictures)
            .FirstOrDefaultAsync(x => x.Id == roomId, cancellationToken);

        if (room is null)
        {
            logger.LogWarning("Room with id {RoomId} was not found when retrieving images", roomId);
            throw new InvalidOperationException("Room not found.");
        }

        if (room.RoomPictures is null || room.RoomPictures.Count == 0)
        {
            logger.LogInformation("No images found for room {RoomId}", roomId);
            return [];
        }

        logger.LogInformation("Found {Count} images for room {RoomId}", room.RoomPictures.Count, roomId);

        return room.RoomPictures
            .OrderBy(x => x.Id)
            .Select(x => new RoomPictures { Id = x.Id, Path = x.Name })
            .ToList();
    }

    public async Task<bool> DeleteImageAsync(int roomId, int imageId, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Attempting to delete image {ImageId} for room {RoomId}", imageId, roomId);

        if (roomId <= 0 || imageId <= 0)
        {
            logger.LogWarning("Invalid parameters for DeleteImageAsync. RoomId: {RoomId}, ImageId: {ImageId}", roomId, imageId);
            return false;
        }

        Room? room = await flexiRoomsContext.Rooms
            .Include(x => x.RoomPictures)
            .FirstOrDefaultAsync(x => x.Id == roomId, cancellationToken);

        if (room?.RoomPictures is null)
        {
            logger.LogWarning("Room {RoomId} or its pictures not found", roomId);
            return false;
        }

        RoomPicture? picture = room.RoomPictures.FirstOrDefault(x => x.Id == imageId);
        if (picture is null)
        {
            logger.LogWarning("Picture {ImageId} not found in room {RoomId}", imageId, roomId);
            return false;
        }

        room.RoomPictures.Remove(picture);
        flexiRoomsContext.RoomPictures.Remove(picture);

        if (!string.IsNullOrWhiteSpace(picture.Name))
        {
            string relativePath = picture.Name.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
            string physicalPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relativePath);
            if (File.Exists(physicalPath))
            {
                try
                {
                    File.Delete(physicalPath);
                    logger.LogInformation("Deleted physical file {PhysicalPath}", physicalPath);
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Could not delete physical file {PhysicalPath}", physicalPath);
                }
            }
        }

        await flexiRoomsContext.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Successfully deleted image {ImageId} from room {RoomId}", imageId, roomId);
        return true;
    }

    private static string NormalizeRoomName(string? roomName, int roomId)
    {
        string baseName = string.IsNullOrWhiteSpace(roomName)
            ? $"room-{roomId}"
            : roomName.Trim().ToLowerInvariant();

        string normalized = Regex.Replace(baseName, "[^a-z0-9]+", "-").Trim('-');

        return string.IsNullOrWhiteSpace(normalized)
            ? $"room-{roomId}"
            : normalized;
    }

    private static int GetCurrentIndex(ICollection<RoomPicture>? roomPictures, string normalizedRoomName)
    {
        if (roomPictures is null || roomPictures.Count == 0)
            return 0;

        string pattern = $"^{Regex.Escape(normalizedRoomName)}-(\\d+)$";
        Regex regex = new(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);

        int maxIndex = 0;

        foreach (RoomPicture picture in roomPictures)
        {
            if (string.IsNullOrWhiteSpace(picture.Name))
                continue;

            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(picture.Name);
            Match match = regex.Match(fileNameWithoutExtension);

            if (!match.Success)
                continue;

            if (int.TryParse(match.Groups[1].Value, out int parsedIndex) && parsedIndex > maxIndex)
                maxIndex = parsedIndex;
        }

        return maxIndex;
    }

    private static void ValidateFile(RoomImageUploadFile file)
    {
        if (string.IsNullOrWhiteSpace(file.FileName))
            throw new ArgumentException("Image file name is required.", nameof(file));

        if (file.Content is null || file.Content.Length == 0)
            throw new ArgumentException("Image content is required.", nameof(file));

        if (file.Content.Length > MaxFileSizeBytes)
            throw new ArgumentException("Image file exceeds maximum allowed size.", nameof(file));

        string extension = Path.GetExtension(file.FileName);
        if (string.IsNullOrWhiteSpace(extension) || !AllowedExtensions.Contains(extension))
            throw new ArgumentException("Image format is not supported.", nameof(file));
    }
}
