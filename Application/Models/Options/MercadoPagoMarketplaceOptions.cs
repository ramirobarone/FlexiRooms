namespace Application.Models.Options
{
    /// <summary>
    /// Mercado Pago Marketplace settings (section "MercadoPago").
    /// Secrets (ClientSecret, TokenEncryptionKey, Token) must come from environment
    /// variables / user-secrets, never from a committed appsettings file.
    /// </summary>
    public class MercadoPagoMarketplaceOptions
    {
        public const string SectionName = "MercadoPago";

        /// <summary>Marketplace application id (panel de developers → Tus integraciones).</summary>
        public string? ClientId { get; set; }

        /// <summary>Marketplace application secret. Secret.</summary>
        public string? ClientSecret { get; set; }

        /// <summary>
        /// Absolute callback URL registered in the Mercado Pago application, e.g.
        /// https://flexirooms.com/api/MercadoPagoAccounts/oauth/callback
        /// </summary>
        public string? OAuthRedirectUri { get; set; }

        public string AuthorizationUrl { get; set; } = "https://auth.mercadopago.com/authorization";
        public string ApiBaseUrl { get; set; } = "https://api.mercadopago.com";

        /// <summary>Where the browser lands after connecting (SPA route). Relative is fine when the SPA is served by the API.</summary>
        public string AdminReturnUrl { get; set; } = "/admin";

        /// <summary>Platform commission charged on every payment, in percent (e.g. 5 = 5%). 0 disables it.</summary>
        public decimal MarketplaceFeePercent { get; set; }

        /// <summary>Base64 of 32 random bytes used to encrypt seller tokens at rest (AES-256-GCM). Secret.</summary>
        public string? TokenEncryptionKey { get; set; }

        /// <summary>Webhook URL sent with each payment so sellers' payment notifications reach this API.</summary>
        public string? NotificationUrl { get; set; }

        /// <summary>
        /// When a hotel has no connected account, charge with the platform account instead of failing.
        /// Keep false in production: otherwise the hotel's money lands in the platform account.
        /// </summary>
        public bool UsePlatformAccountWhenHotelNotConnected { get; set; }

        /// <summary>Platform (marketplace owner) public key, only used by the fallback above.</summary>
        public string? PublicKey { get; set; }

        /// <summary>Platform access token, only used by the fallback above and legacy webhooks. Secret.</summary>
        public string? Token { get; set; }

        public bool IsMarketplaceConfigured =>
            !string.IsNullOrWhiteSpace(ClientId)
            && !string.IsNullOrWhiteSpace(ClientSecret)
            && !string.IsNullOrWhiteSpace(OAuthRedirectUri)
            && !string.IsNullOrWhiteSpace(TokenEncryptionKey);
    }
}
