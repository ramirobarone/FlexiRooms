using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? Name { get; set; }
        public string? SecondName { get; set; }
        public string? LastName { get; set; }
        public string? IdentityNumber { get; set; }
        public string? CodeArea { get; set; }
        public bool AccountActivate { get; set; }
        public Guid UserGuid { get; set; }
        public bool IsOwnAccount { get; set; }
        public bool ProviderAccount { get; set; }
        public int? ManagedHotelId { get; set; }
        public string? OwnedHotelIds { get; set; }
    }
}
