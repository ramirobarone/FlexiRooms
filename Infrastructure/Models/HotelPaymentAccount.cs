using System.ComponentModel.DataAnnotations.Schema;

namespace Infrastructure.Models
{
    /// <summary>
    /// Mercado Pago seller account connected to a hotel through the Marketplace OAuth flow.
    /// Payments for the hotel's rooms are created with this account's access token, so the
    /// money is credited directly to the hotel. Tokens are stored encrypted (AES-GCM).
    /// </summary>
    public class HotelPaymentAccount
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int HotelId { get; set; }
        public Hotel Hotel { get; set; } = null!;

        /// <summary>Mercado Pago user id of the seller (collector).</summary>
        public long MercadoPagoUserId { get; set; }

        /// <summary>Public key used by the Checkout Brick in the browser. Not secret.</summary>
        public required string PublicKey { get; set; }

        public required string AccessTokenEncrypted { get; set; }
        public required string RefreshTokenEncrypted { get; set; }

        public DateTime AccessTokenExpiresAtUtc { get; set; }
        public bool LiveMode { get; set; }

        /// <summary>AspNetUsers.Id of the person who authorized the connection.</summary>
        public string? ConnectedByUserId { get; set; }
        public DateTime ConnectedAtUtc { get; set; }
        public DateTime UpdatedAtUtc { get; set; }
    }
}
