using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Models
{
    public class Cost
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public decimal CostPerTime { get; set; }
        public int Hour { get; set; }
        public int RoomId { get; set; }
    }
}