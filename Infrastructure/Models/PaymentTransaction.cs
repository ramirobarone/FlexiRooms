using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Models
{
    public class PaymentTransaction
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public required string MercadoPagoPaymentId { get; set; }
        public required string IdempotencyKey { get; set; }
        public required string Status { get; set; }
        public string? StatusDetail { get; set; }
        public required string CurrencyId { get; set; }
        public required string PaymentMethodId { get; set; }
        public int Installments { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public int? BookingId { get; set; }
        public required string ApplicationUserId { get; set; }

        public Bookings Booking { get; set; } = null!;
        public ApplicationUser ApplicationUser { get; set; } = null!;
    }
}