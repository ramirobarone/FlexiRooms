using Application.Interfaces;
using Application.Models;
using Infrastructure.Context;
using Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Services.Maintenance;

public class HotelMaintenanceService(FlexiRoomsContext context, ILogger<HotelMaintenanceService> logger) : IHotelMaintenanceService
{
    public async Task<IReadOnlyCollection<HotelMaintenanceDto>> GetByHotelIdAsync(int hotelId, CancellationToken cancellationToken = default)
    {
        if (hotelId <= 0)
            throw new ArgumentException("Hotel id is invalid.", nameof(hotelId));

        List<HotelMaintenance> maintenances = await context.HotelMaintenances
            .AsNoTracking()
            .Include(x => x.MaintenanceTypes)
            .Where(x => x.HotelId == hotelId)
            .OrderBy(x => x.NameMaintenance)
            .ToListAsync(cancellationToken);

        logger.LogInformation("Retrieved {Count} maintenances for hotel {HotelId}", maintenances.Count, hotelId);
        return maintenances.Select(x => (HotelMaintenanceDto)x).ToList();
    }

    public async Task<HotelMaintenanceDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        if (id <= 0)
            throw new ArgumentException("Maintenance id is invalid.", nameof(id));

        HotelMaintenance maintenance = await GetMaintenanceOrThrowAsync(id, cancellationToken);
        logger.LogInformation("Retrieved maintenance {MaintenanceId}", id);
        return maintenance;
    }

    public async Task<HotelMaintenanceDto> CreateAsync(CreateHotelMaintenanceDto request, string userId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ValidateUserId(userId);
        await ValidateHotelAsync(request.HotelId, cancellationToken);
        List<MaintenanceType> maintenanceTypes = await ValidateMaintenanceTypesAsync(request.MaintenanceTypeIds, cancellationToken);

        if (string.IsNullOrWhiteSpace(request.NameMaintenance))
            throw new ArgumentException("Maintenance name is required.", nameof(request.NameMaintenance));

        if (string.IsNullOrWhiteSpace(request.TelephoneNumber))
            throw new ArgumentException("Telephone number is required.", nameof(request.TelephoneNumber));

        HotelMaintenance maintenance = new()
        {
            NameMaintenance = request.NameMaintenance.Trim(),
            HotelId = request.HotelId,
            TelephoneNumber = request.TelephoneNumber.Trim(),
            Active = request.Active,
            CreateDate = DateTime.UtcNow,
            CreateBy = userId,
            MaintenanceTypes = maintenanceTypes
        };

        context.HotelMaintenances.Add(maintenance);
        await context.SaveChangesAsync(cancellationToken);

        HotelMaintenance created = await GetMaintenanceOrThrowAsync(maintenance.Id, cancellationToken);
        logger.LogInformation("Created maintenance {MaintenanceId} for hotel {HotelId}", created.Id, created.HotelId);
        return created;
    }

    public async Task<HotelMaintenanceDto> UpdateAsync(UpdateHotelMaintenanceDto request, string userId, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ValidateUserId(userId);

        if (request.Id <= 0)
            throw new ArgumentException("Maintenance id is invalid.", nameof(request.Id));

        await ValidateHotelAsync(request.HotelId, cancellationToken);
        List<MaintenanceType> maintenanceTypes = await ValidateMaintenanceTypesAsync(request.MaintenanceTypeIds, cancellationToken);

        if (string.IsNullOrWhiteSpace(request.NameMaintenance))
            throw new ArgumentException("Maintenance name is required.", nameof(request.NameMaintenance));

        if (string.IsNullOrWhiteSpace(request.TelephoneNumber))
            throw new ArgumentException("Telephone number is required.", nameof(request.TelephoneNumber));

        HotelMaintenance maintenance = await context.HotelMaintenances
            .Include(x => x.MaintenanceTypes)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new InvalidOperationException("Maintenance not found.");

        maintenance.NameMaintenance = request.NameMaintenance.Trim();
        maintenance.HotelId = request.HotelId;
        maintenance.TelephoneNumber = request.TelephoneNumber.Trim();
        maintenance.Active = request.Active;
        maintenance.UpdateDate = DateTime.UtcNow;
        maintenance.UpdateBy = userId;
        maintenance.MaintenanceTypes.Clear();
        foreach (MaintenanceType maintenanceType in maintenanceTypes)
            maintenance.MaintenanceTypes.Add(maintenanceType);

        await context.SaveChangesAsync(cancellationToken);

        HotelMaintenance updated = await GetMaintenanceOrThrowAsync(maintenance.Id, cancellationToken);
        logger.LogInformation("Updated maintenance {MaintenanceId}", updated.Id);
        return updated;
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        if (id <= 0)
            throw new ArgumentException("Maintenance id is invalid.", nameof(id));

        HotelMaintenance maintenance = await context.HotelMaintenances
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new InvalidOperationException("Maintenance not found.");

        context.HotelMaintenances.Remove(maintenance);
        await context.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Deleted maintenance {MaintenanceId}", id);
    }

    public async Task<IReadOnlyCollection<MaintenanceTypeDto>> GetMaintenanceTypesAsync(CancellationToken cancellationToken = default)
    {
        List<MaintenanceType> maintenanceTypes = await context.MaintenanceTypes
            .AsNoTracking()
            .OrderBy(x => x.Description)
            .ToListAsync(cancellationToken);

        return maintenanceTypes.Select(x => (MaintenanceTypeDto)x).ToList();
    }

    private async Task<HotelMaintenance> GetMaintenanceOrThrowAsync(int id, CancellationToken cancellationToken)
    {
        return await context.HotelMaintenances
            .AsNoTracking()
            .Include(x => x.MaintenanceTypes)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new InvalidOperationException("Maintenance not found.");
    }

    private async Task ValidateHotelAsync(int hotelId, CancellationToken cancellationToken)
    {
        if (hotelId <= 0)
            throw new ArgumentException("Hotel id is invalid.", nameof(hotelId));

        bool hotelExists = await context.Hotels.AnyAsync(x => x.Id == hotelId, cancellationToken);
        if (!hotelExists)
            throw new ArgumentException("Hotel does not exist.", nameof(hotelId));
    }

    private async Task<List<MaintenanceType>> ValidateMaintenanceTypesAsync(IReadOnlyCollection<int> maintenanceTypeIds, CancellationToken cancellationToken)
    {
        List<int> distinctIds = (maintenanceTypeIds ?? []).Distinct().ToList();
        if (distinctIds.Count == 0)
            throw new ArgumentException("At least one maintenance type is required.", nameof(maintenanceTypeIds));

        List<MaintenanceType> maintenanceTypes = await context.MaintenanceTypes
            .Where(x => distinctIds.Contains(x.Id))
            .ToListAsync(cancellationToken);

        if (maintenanceTypes.Count != distinctIds.Count)
            throw new ArgumentException("One or more maintenance types do not exist.", nameof(maintenanceTypeIds));

        return maintenanceTypes;
    }

    private static void ValidateUserId(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("User id is invalid.", nameof(userId));
    }
}
