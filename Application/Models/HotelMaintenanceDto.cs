using Infrastructure.Models;

namespace Application.Models
{
    public class HotelMaintenanceDto(
        int id,
        string nameMaintenance,
        int hotelId,
        IReadOnlyCollection<MaintenanceTypeDto> maintenanceTypes,
        string telephoneNumber,
        bool active,
        DateTime createDate,
        string createBy,
        DateTime? updateDate,
        string? updateBy)
    {
        public int Id { get; } = id;
        public string NameMaintenance { get; } = nameMaintenance;
        public int HotelId { get; } = hotelId;
        public IReadOnlyCollection<MaintenanceTypeDto> MaintenanceTypes { get; } = maintenanceTypes;
        public string TelephoneNumber { get; } = telephoneNumber;
        public bool Active { get; } = active;
        public DateTime CreateDate { get; } = createDate;
        public string CreateBy { get; } = createBy;
        public DateTime? UpdateDate { get; } = updateDate;
        public string? UpdateBy { get; } = updateBy;

        public static implicit operator HotelMaintenanceDto(HotelMaintenance maintenance)
        {
            return new HotelMaintenanceDto(
                maintenance.Id,
                maintenance.NameMaintenance,
                maintenance.HotelId,
                maintenance.MaintenanceTypes.Select(x => (MaintenanceTypeDto)x).ToList(),
                maintenance.TelephoneNumber,
                maintenance.Active,
                maintenance.CreateDate,
                maintenance.CreateBy,
                maintenance.UpdateDate,
                maintenance.UpdateBy);
        }
    }

    public class CreateHotelMaintenanceDto
    {
        public string? NameMaintenance { get; set; }
        public int HotelId { get; set; }
        public List<int> MaintenanceTypeIds { get; set; } = [];
        public string? TelephoneNumber { get; set; }
        public bool Active { get; set; }
    }

    public class UpdateHotelMaintenanceDto
    {
        public int Id { get; set; }
        public string? NameMaintenance { get; set; }
        public int HotelId { get; set; }
        public List<int> MaintenanceTypeIds { get; set; } = [];
        public string? TelephoneNumber { get; set; }
        public bool Active { get; set; }
    }
}
