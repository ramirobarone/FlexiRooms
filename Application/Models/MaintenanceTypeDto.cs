using Infrastructure.Models;

namespace Application.Models
{
    public class MaintenanceTypeDto(int id, string description)
    {
        public int Id { get; } = id;
        public string Description { get; } = description;

        public static implicit operator MaintenanceTypeDto(MaintenanceType maintenanceType)
        {
            return new MaintenanceTypeDto(maintenanceType.Id, maintenanceType.Description);
        }
    }
}
