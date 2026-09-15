using Infrastructure.Models;

namespace Application.Models.Booking
{
    public class BookingInputDto
    {
        public required int IdRoom { get; set; }
        public required DateTime Date { get; set; }
        public required int CheckInTimeId { get;set;}

        public Guid UserGuid { get; set; }

        public static implicit operator Bookings(BookingInputDto dto)
        {
            ArgumentNullException.ThrowIfNull(dto);

            return new Bookings() { IdRoom = dto.IdRoom, DateReserved = NormalizeToUtc(dto.Date), CheckInTimeId = dto.CheckInTimeId, UserGuid = dto.UserGuid };
        }

        private static DateTime NormalizeToUtc(DateTime value)
            => value.Kind switch
            {
                DateTimeKind.Utc => value,
                DateTimeKind.Local => value.ToUniversalTime(),
                DateTimeKind.Unspecified => DateTime.SpecifyKind(value, DateTimeKind.Utc),
                _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
            };

        public override string ToString()
        {
            return string.Format("IdRoom: {0}, Date {1}, TimeId {2}", IdRoom, Date, CheckInTimeId);
        }
    }
}