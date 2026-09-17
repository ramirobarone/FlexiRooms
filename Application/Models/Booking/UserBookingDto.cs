namespace Application.Models.Booking
{
    public record UserBookingDto(
        int Id,
        int RoomId,
        DateTime StartDate,
        DateTime EndDate,
        string StartTime,
        string EndTime,
        string PaymentStatus,
        string? TerminosYCondiciones,
        string? InstruccionesDeUso,
        string? HotelWhatsAppNumber);
}