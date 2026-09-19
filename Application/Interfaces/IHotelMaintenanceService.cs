using Application.Models;

namespace Application.Interfaces;

public interface IHotelMaintenanceService
{
    Task<IReadOnlyCollection<HotelMaintenanceDto>> GetByHotelIdAsync(int hotelId, CancellationToken cancellationToken = default);
    Task<HotelMaintenanceDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<HotelMaintenanceDto> CreateAsync(CreateHotelMaintenanceDto request, string userId, CancellationToken cancellationToken = default);
    Task<HotelMaintenanceDto> UpdateAsync(UpdateHotelMaintenanceDto request, string userId, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<MaintenanceTypeDto>> GetMaintenanceTypesAsync(CancellationToken cancellationToken = default);
}
