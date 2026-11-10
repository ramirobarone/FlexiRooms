namespace Application.Interfaces
{
    public sealed record HotelPaymentAccountStatus(
        int HotelId,
        bool Connected,
        long? MercadoPagoUserId,
        bool LiveMode,
        DateTime? ConnectedAtUtc);

    /// <summary>Credentials to charge on behalf of a seller. AccessToken is secret: never log or return it.</summary>
    public sealed record SellerCredentials(
        int? HotelId,
        string AccessToken,
        string PublicKey,
        bool IsPlatformAccount);

    public interface IMercadoPagoAccountService
    {
        bool IsMarketplaceConfigured { get; }

        Task<bool> CanManageHotelAsync(int hotelId, string userId, bool isSuperAdmin, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<HotelPaymentAccountStatus>> GetStatusesForOwnerAsync(string userId, CancellationToken cancellationToken = default);

        Task<HotelPaymentAccountStatus> GetStatusAsync(int hotelId, CancellationToken cancellationToken = default);

        /// <summary>Mercado Pago authorization URL with a signed, short-lived state bound to hotel + user.</summary>
        string BuildAuthorizationUrl(int hotelId, string userId);

        /// <summary>Validates the state, exchanges the code and stores the seller account. Returns the hotel id.</summary>
        Task<int> CompleteAuthorizationAsync(string code, string state, CancellationToken cancellationToken = default);

        /// <summary>Best-effort hotel id from a state value, for redirecting after an error. Null if invalid.</summary>
        int? TryReadHotelIdFromState(string? state);

        Task DisconnectAsync(int hotelId, CancellationToken cancellationToken = default);

        /// <summary>Seller credentials for a hotel (refreshing the token if needed), or the platform account when the fallback is enabled.</summary>
        Task<SellerCredentials?> GetCredentialsForHotelAsync(int hotelId, CancellationToken cancellationToken = default);

        /// <summary>Seller credentials by Mercado Pago user id (used by webhooks).</summary>
        Task<SellerCredentials?> GetCredentialsForCollectorAsync(long mercadoPagoUserId, CancellationToken cancellationToken = default);

        /// <summary>Platform commission for an amount, or null when no fee is configured.</summary>
        decimal? CalculateApplicationFee(decimal amount);
    }
}
