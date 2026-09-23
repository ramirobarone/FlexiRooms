using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Models
{
    public class Issue
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int? MaintenanceTypeId { get; set; }
        public required string Text { get; set; }
        public string? Image { get; set; }
        public string Status { get; set; } = "Pending";
        public required string ApplicationUserId { get; set; }
        public int? BookingId { get; set; }
        public DateTime CreatedAtUtc { get; set; }

        public MaintenanceType? MaintenanceType { get; set; }
        public ApplicationUser ApplicationUser { get; set; } = null!;
        public Bookings? Booking { get; set; }
    }
}
