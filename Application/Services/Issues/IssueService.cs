using Application.Interfaces;
using Application.Models;
using Infrastructure.Context;
using Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Issues;

public class IssueService(FlexiRoomsContext flexiRoomsContext) : IIssueService
{
    private static readonly HashSet<string> AllowedStatuses = new(StringComparer.OrdinalIgnoreCase) { "Resuelto", "En Proceso", "Anulado" };
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".webp"
    };

    private const int MaxFileSizeBytes = 5 * 1024 * 1024;

    public async Task<IssueDto> CreateIssueAsync(string applicationUserId, int bookingId, int tipoDeReclamo, string texto, IssueUploadFile? image, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(applicationUserId))
            throw new ArgumentException("User id is invalid.", nameof(applicationUserId));

        ApplicationUser? user = await flexiRoomsContext.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == applicationUserId, cancellationToken);
        if (bookingId <= 0 || user is null || !await flexiRoomsContext.Bookings.AnyAsync(x => x.Id == bookingId && x.UserGuid == user.UserGuid, cancellationToken))
            throw new ArgumentException("La reserva indicada no pertenece al usuario.", nameof(bookingId));

        if (string.IsNullOrWhiteSpace(texto))
            throw new ArgumentException("El texto del reclamo es obligatorio.", nameof(texto));

        bool issueTypeExists = await flexiRoomsContext.IssuesTypes.AnyAsync(x => x.Id == tipoDeReclamo, cancellationToken);
        if (!issueTypeExists)
            throw new ArgumentException("El tipo de reclamo indicado no existe.", nameof(tipoDeReclamo));

        Issue issue = new()
        {
            MaintenanceTypeId = tipoDeReclamo,
            Text = texto,
            Status = "Pendiente",
            ApplicationUserId = applicationUserId,
            BookingId = bookingId,
            CreatedAtUtc = DateTime.UtcNow
        };

        if (image is not null)
        {
            ValidateFile(image);

            string issuesDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "issues");
            Directory.CreateDirectory(issuesDirectory);

            string extension = Path.GetExtension(image.FileName).ToLowerInvariant();
            string newFileName = $"{Guid.NewGuid()}{extension}";
            string physicalPath = Path.Combine(issuesDirectory, newFileName);

            await File.WriteAllBytesAsync(physicalPath, image.Content, cancellationToken);

            issue.Image = $"/issues/{newFileName}";
        }

        flexiRoomsContext.Issues.Add(issue);
        await flexiRoomsContext.SaveChangesAsync(cancellationToken);

        Issue created = await flexiRoomsContext.Issues
            .Include(x => x.MaintenanceType)
            .FirstAsync(x => x.Id == issue.Id, cancellationToken);

        return created;
    }

    public async Task<IReadOnlyCollection<IssueDto>> GetMyIssuesAsync(string applicationUserId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(applicationUserId))
            throw new ArgumentException("User id is invalid.", nameof(applicationUserId));

        List<Issue> issues = await flexiRoomsContext.Issues
            .Include(x => x.MaintenanceType)
            .Where(x => x.ApplicationUserId == applicationUserId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);

        return issues.Select(x => (IssueDto)x).ToList();
    }

    public async Task<IReadOnlyCollection<IssueDto>> GetHotelIssuesAsync(string applicationUserId, bool isSuperAdmin, CancellationToken cancellationToken = default)
    {
        HashSet<int> hotelIds = await GetAccessibleHotelIdsAsync(applicationUserId, isSuperAdmin, cancellationToken);
        List<Issue> issues = await flexiRoomsContext.Issues
            .Include(x => x.MaintenanceType)
            .Include(x => x.Booking).ThenInclude(x => x!.Room).ThenInclude(x => x!.Hotels)
            .Where(x => x.Booking != null && x.Booking.Room != null && x.Booking.Room.Hotels != null && hotelIds.Contains(x.Booking.Room.Hotels.Id))
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);
        return issues.Select(x => (IssueDto)x).ToList();
    }

    public async Task<IssueDto> UpdateHotelIssueStatusAsync(string applicationUserId, bool isSuperAdmin, int issueId, string status, CancellationToken cancellationToken = default)
    {
        string normalizedStatus = status?.Trim() ?? string.Empty;
        if (issueId <= 0 || !AllowedStatuses.Contains(normalizedStatus))
            throw new ArgumentException("El estado o reclamo indicado no es válido.");

        HashSet<int> hotelIds = await GetAccessibleHotelIdsAsync(applicationUserId, isSuperAdmin, cancellationToken);
        Issue? issue = await flexiRoomsContext.Issues
            .Include(x => x.MaintenanceType)
            .Include(x => x.Booking).ThenInclude(x => x!.Room).ThenInclude(x => x!.Hotels)
            .FirstOrDefaultAsync(x => x.Id == issueId, cancellationToken);
        if (issue?.Booking?.Room?.Hotels is null || !hotelIds.Contains(issue.Booking.Room.Hotels.Id))
            throw new UnauthorizedAccessException();

        issue.Status = normalizedStatus;
        await flexiRoomsContext.SaveChangesAsync(cancellationToken);
        return issue;
    }

    public async Task<IReadOnlyCollection<IssueTypeDto>> GetIssueTypesAsync(CancellationToken cancellationToken = default)
    {
        List<IssueType> issueTypes = await flexiRoomsContext.IssuesTypes
            .OrderBy(x => x.Issue)
            .ToListAsync(cancellationToken);

        return issueTypes.Select(x => (IssueTypeDto)x).ToList();
    }

    private async Task<HashSet<int>> GetAccessibleHotelIdsAsync(string applicationUserId, bool isSuperAdmin, CancellationToken cancellationToken)
    {
        if (isSuperAdmin)
            return (await flexiRoomsContext.Hotels.Select(x => x.Id).ToListAsync(cancellationToken)).ToHashSet();

        ApplicationUser? user = await flexiRoomsContext.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == applicationUserId, cancellationToken);
        if (user is null)
            return [];

        HashSet<int> hotelIds = user.ManagedHotelId.HasValue ? [user.ManagedHotelId.Value] : [];
        hotelIds.UnionWith((user.OwnedHotelIds ?? string.Empty).Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(value => int.TryParse(value.Trim(), out int hotelId) ? hotelId : 0).Where(hotelId => hotelId > 0));
        hotelIds.UnionWith(await flexiRoomsContext.Hotels.Where(x => x.IdentityNumber == applicationUserId).Select(x => x.Id).ToListAsync(cancellationToken));
        return hotelIds;
    }

    private static void ValidateFile(IssueUploadFile file)
    {
        if (string.IsNullOrWhiteSpace(file.FileName))
            throw new ArgumentException("El nombre del archivo es obligatorio.", nameof(file));

        if (file.Content is null || file.Content.Length == 0)
            throw new ArgumentException("El contenido de la imagen es obligatorio.", nameof(file));

        if (file.Content.Length > MaxFileSizeBytes)
            throw new ArgumentException("La imagen supera el tamaño máximo permitido.", nameof(file));

        string extension = Path.GetExtension(file.FileName);
        if (string.IsNullOrWhiteSpace(extension) || !AllowedExtensions.Contains(extension))
            throw new ArgumentException("El formato de imagen no es compatible.", nameof(file));
    }
}
