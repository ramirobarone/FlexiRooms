using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ClasificatorMaintenance.Database
{
    internal class Issue
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int MaintenanceTypeId { get; set; }
        public required string Text { get; set; }
        public string? Image { get; set; }
        public string Status { get; set; } = "Pending";
        public int? BookingId { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }
}
