using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Models
{
    public class Address
    {
        public int Id { get; set; }
        public string? Street { get; set; }
        public string? Number { get; set; }
        public string? PostalCode { get; set; }
        public string? Latitud { get; set; }
        public string? Longitud { get; set; }
        public int ProvinceId { get; set; }
        public int IdCity { get; set; }
        public int CountryId { get; set; }
    }
}