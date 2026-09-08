using System.Globalization;
using System.Text.RegularExpressions;
using Application.Interfaces;
using Application.Models;
using Infrastructure.Context;
using Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Services.HotelServices;

public class HotelImageService(FlexiRoomsContext flexiRoomsContext,
                               ILogger<HotelImageService> logger) : IHotelImageService
{
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".webp"
    };

    private const int MaxFileSizeBytes = 5 * 1024 * 1024;

    public async Task<IReadOnlyCollection<HotelPictureDto>> UploadImagesAsync(int hotelId, IReadOnlyCollection<HotelImageUploadFile> files, CancellationToken cancellationToken = default)
    {
        if (hotelId <= 0)
            throw new ArgumentException("Hotel id is invalid.", nameof(hotelId));

        ArgumentNullException.ThrowIfNull(files);

        if (files.Count == 0)
            throw new ArgumentException("At least one image is required.", nameof(files));

        Hotel? hotel = await flexiRoomsContext.Hotels
            .Include(x => x.HotelPictures)
            .FirstOrDefaultAsync(x => x.Id == hotelId, cancellationToken);

        if (hotel is null)
            throw new InvalidOperationException("Hotel not found.");

        string normalizedHotelName = NormalizeHotelName(hotel.Name, hotel.Id);
        string imagesDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "hotel-images", hotelId.ToString(CultureInfo.InvariantCulture));
        Directory.CreateDirectory(imagesDirectory);

        int currentIndex = GetCurrentIndex(hotel.HotelPictures, normalizedHotelName);

        hotel.HotelPictures ??= new List<HotelPicture>();
        List<HotelPicture> uploadedImages = new(files.Count);

        foreach (HotelImageUploadFile file in files)
        {
            ValidateFile(file);

            currentIndex++;
            string extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            string newFileName = $"{normalizedHotelName}-{currentIndex}{extension}";
            string physicalPath = Path.Combine(imagesDirectory, newFileName);

            while (File.Exists(physicalPath))
            {
                currentIndex++;
                newFileName = $"{normalizedHotelName}-{currentIndex}{extension}";
                physicalPath = Path.Combine(imagesDirectory, newFileName);
            }

            await File.WriteAllBytesAsync(physicalPath, file.Content, cancellationToken);

            string relativePath = $"/hotel-images/{hotelId}/{newFileName}";
            HotelPicture hotelPicture = new()
            {
                Path = relativePath
            };

            hotel.HotelPictures.Add(hotelPicture);
            uploadedImages.Add(hotelPicture);
        }

        await flexiRoomsContext.SaveChangesAsync(cancellationToken);

        return uploadedImages.Select(x => (HotelPictureDto)x).ToList();
    }

    public async Task<IReadOnlyCollection<HotelPictureDto>> GetHotelImagesAsync(int hotelId, CancellationToken cancellationToken = default)
    {
        if (hotelId <= 0)
            throw new ArgumentException("Hotel id is invalid.", nameof(hotelId));

        Hotel? hotel = await flexiRoomsContext.Hotels
            .Include(x => x.HotelPictures)
            .FirstOrDefaultAsync(x => x.Id == hotelId, cancellationToken);

        if (hotel is null)
            throw new InvalidOperationException("Hotel not found.");

        if (hotel.HotelPictures is null || hotel.HotelPictures.Count == 0)
            return [];

        return hotel.HotelPictures
            .OrderBy(x => x.Id)
            .Select(x => (HotelPictureDto)x)
            .ToList();
    }

    private static string NormalizeHotelName(string? hotelName, int hotelId)
    {
        string baseName = string.IsNullOrWhiteSpace(hotelName)
            ? $"hotel-{hotelId}"
            : hotelName.Trim().ToLowerInvariant();

        string normalized = Regex.Replace(baseName, "[^a-z0-9]+", "-").Trim('-');

        return string.IsNullOrWhiteSpace(normalized)
            ? $"hotel-{hotelId}"
            : normalized;
    }

    private static int GetCurrentIndex(ICollection<HotelPicture>? hotelPictures, string normalizedHotelName)
    {
        if (hotelPictures is null || hotelPictures.Count == 0)
            return 0;

        string pattern = $"^{Regex.Escape(normalizedHotelName)}-(\\d+)$";
        Regex regex = new(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);

        int maxIndex = 0;

        foreach (HotelPicture picture in hotelPictures)
        {
            if (string.IsNullOrWhiteSpace(picture.Path))
                continue;

            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(picture.Path);
            Match match = regex.Match(fileNameWithoutExtension);

            if (!match.Success)
                continue;

            if (int.TryParse(match.Groups[1].Value, out int parsedIndex) && parsedIndex > maxIndex)
                maxIndex = parsedIndex;
        }

        return maxIndex;
    }

    private static void ValidateFile(HotelImageUploadFile file)
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
