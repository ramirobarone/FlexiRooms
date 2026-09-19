using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Models
{
    public class MaintenanceType
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public required string Description { get; set; }
        public ICollection<HotelMaintenance> HotelMaintenances { get; set; } = new List<HotelMaintenance>();
    }
}
