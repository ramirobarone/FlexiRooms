using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Models
{
    public class HotelMaintenance
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public required string NameMaintenance { get; set; }
        public int HotelId { get; set; }
        public required string TelephoneNumber { get; set; }
        public bool Active { get; set; }
        public DateTime CreateDate { get; set; }
        public required string CreateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string? UpdateBy { get; set; }

        public Hotel? Hotel { get; set; }
        public ICollection<MaintenanceType> MaintenanceTypes { get; set; } = new List<MaintenanceType>();
    }
}
