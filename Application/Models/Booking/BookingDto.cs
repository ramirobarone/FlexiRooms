using Infrastructure.Models;

namespace Application.Models.Booking
{
    public class BookingDto : BookingBase
    {
        public int Id { get; set; }
        public int CheckInTimeId { get; set; }
        public int IdRoom { get; set; }
        public decimal Price { get; set; }
        public Guid UserGuid { get; set; }
        public DateTime DateReserved { get; set; }
        public virtual TimesAvailable? CheckInTime { get; set; }
    }
}
