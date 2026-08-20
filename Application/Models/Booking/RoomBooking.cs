namespace Application.Models.Booking
{
    public record RoomBooking(int IdRoom, DateTime Date, int CheckInTimeId, string UserGuid, int Price);
}
