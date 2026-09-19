using System.ComponentModel.DataAnnotations.Schema;

namespace ClasificatorMaintenance.Database
{
    public class Hotel
    {
        public int Id { get; set; }
        public int PhoneNumber { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
    }
}