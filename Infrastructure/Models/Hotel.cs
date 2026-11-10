using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Models
{
    public class Hotel
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int CodeArea { get; set; }
        public int PhoneNumber { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? MetaDescription { get; set; }
        public string? Email { get; set; }
        public string? IdentityNumber { get; set; }
        public decimal? ReviewScore { get; set; }
        public Address? AddressHotel { get; set; }
        public virtual ICollection<HotelPicture>? HotelPictures{ get; set; }

        /// <summary>FK toward AspNetUsers.Id — the user who owns this hotel.</summary>
        public ApplicationUser? Owner { get; set; }

        /// <summary>Only Approved hotels are listed publicly and accept payments.</summary>
        public HotelApprovalStatus ApprovalStatus { get; set; }

        /// <summary>Shown to the owner when the hotel is rejected.</summary>
        public string? RejectionReason { get; set; }

        public DateTime? SubmittedAtUtc { get; set; }
        public DateTime? ReviewedAtUtc { get; set; }
        public string? ReviewedByUserId { get; set; }
    }
}