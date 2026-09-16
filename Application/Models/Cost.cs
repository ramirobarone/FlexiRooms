using Infrastructure.Models;

namespace Application.Models
{
    public class CostDto
    {
        public int Id { get; set; }
        public int IdRoom { get; set; }
        public decimal CostPerHour { get; set; }
        public int Hour { get; set; }

        public static implicit operator CostDto(Cost cost)
        {
            return new CostDto { Id = cost.Id, IdRoom = cost.RoomId, CostPerHour = cost.CostPerTime, Hour = cost.Hour };
        }
    }
}