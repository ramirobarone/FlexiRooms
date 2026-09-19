using System.ComponentModel.DataAnnotations.Schema;

namespace ClasificatorMaintenance.Database  
{
    public class MaintenanceType
    {
        public int Id { get; set; }
        public required string Description { get; set; }
    }
}
