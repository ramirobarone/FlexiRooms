using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Models
{
    public class Review
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public required string UserId { get; set; }
        public required string ReviewText { get; set; }
        public int Value { get; set; }
        public int HotelId { get; set; }
        public DateTime CreateDate { get; set; }
        public required string CreateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string? UpdateBy { get; set; }

        public ApplicationUser? User { get; set; }
        public Hotel? Hotel { get; set; }
    }
}
