using System.Globalization;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using Application.Interfaces;
using Application.Models.Options;
using Infrastructure.Context;
using Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Application.Services.Payments
{
    /// <summary>
    /// Mercado Pago Marketplace: connects each hotel's seller account through OAuth and
    /// hands out the credentials needed to charge on its behalf.
    ///
    /// Flow:
    ///  1. Admin clicks "Conectar" → <see cref="BuildAuthorizationUrl"/> (state = signed hotel+user+expiry).
    ///  2. Seller authorizes in Mercado Pago → browser returns to our callback with code + state.
    ///  3. <see cref="CompleteAuthorizationAsync"/> validates state, exchanges the code, stores encrypted tokens.
    ///  4. Checkout / payments / webhooks use <see cref="GetCredentialsForHotelAsync"/> (auto-refresh).
    /// </summary>
    public sealed class MercadoPagoAccountService(
        HttpClient httpClient,
        FlexiRoomsContext context,
        IOptions<MercadoPagoMarketplaceOptions> options,
        ILogger<MercadoPagoAccountService> logger) : IMercadoPagoAccountService
    {
        private static readonly TimeSpan StateLifetime = TimeSpan.FromMinutes(15);
        private static readonly TimeSpan RefreshWhenExpiringWithin = TimeSpan.FromDays(7);

        private readonly MercadoPagoMarketplaceOptions settings = options.Value;

        public bool IsMarketplaceConfigured => settings.IsMarketplaceConfigured;

        // ------------------------------------------------------------ ownership

        public async Task<bool> CanManageHotelAsync(int hotelId, string userId, bool isSuperAdmin, CancellationToken cancellationToken = default)
        {
            if (isSuperAdmin)
            {
                return await context.Hotels.AnyAsync(hotel => hotel.Id == hotelId, cancellationToken);
            }

            return await context.Hotels.AnyAsync(hotel => hotel.Id == hotelId && hotel.IdentityNumber == userId, cancellationToken);
        }

        // --------------------------------------------------------------- status

        public async Task<IReadOnlyList<HotelPaymentAccountStatus>> GetStatusesForOwnerAsync(string userId, CancellationToken cancellationToken = default)
        {
            var hotelIds = await context.Hotels
                .Where(hotel => hotel.IdentityNumber == userId)
                .Select(hotel => hotel.Id)
                .ToListAsync(cancellationToken);

            var accounts = await context.HotelPaymentAccounts
                .Where(account => hotelIds.Contains(account.HotelId))
                .ToDictionaryAsync(account => account.HotelId, cancellationToken);

            return hotelIds
                .Select(id => accounts.TryGetValue(id, out var account) ? ToStatus(account) : NotConnected(id))
                .ToList();
        }

        public async Task<HotelPaymentAccountStatus> GetStatusAsync(int hotelId, CancellationToken cancellationToken = default)
        {
            var account = await context.HotelPaymentAccounts.FirstOrDefaultAsync(a => a.HotelId == hotelId, cancellationToken);
            return account is null ? NotConnected(hotelId) : ToStatus(account);
        }

        // ---------------------------------------------------------------- OAuth

        public string BuildAuthorizationUrl(int hotelId, string userId)
        {
            EnsureMarketplaceConfigured();

            string state = CreateState(hotelId, userId);
            var query = new Dictionary<string, string>
            {
                ["client_id"] = settings.ClientId!,
                ["response_type"] = "code",
                ["platform_id"] = "mp",
                ["state"] = state,
                ["redirect_uri"] = settings.OAuthRedirectUri!
            };

            string queryString = string.Join("&", query.Select(pair => $"{pair.Key}={Uri.EscapeDataString(pair.Value)}"));
            return $"{settings.AuthorizationUrl}?{queryString}";
        }

        public async Task<int> CompleteAuthorizationAsync(string code, string state, CancellationToken cancellationToken = default)
        {
            EnsureMarketplaceConfigured();

            if (!TryReadState(state, out int hotelId, out string userId))
            {
                throw new InvalidOperationException("La solicitud de conexión expiró o no es válida. Volvé a intentarlo.");
            }

            // The user who started the flow must still be allowed to manage the hotel.
            bool isOwner = await context.Hotels.AnyAsync(hotel => hotel.Id == hotelId && hotel.IdentityNumber == userId, cancellationToken);
            bool isSuperAdmin = await IsSuperAdminAsync(userId, cancellationToken);
            if (!isOwner && !isSuperAdmin)
            {
                throw new UnauthorizedAccessException("No tenés permisos para configurar los cobros de este hotel.");
            }

            OAuthTokenResponse token = await RequestTokenAsync(new Dictionary<string, string>
            {
                ["client_id"] = settings.ClientId!,
                ["client_secret"] = settings.ClientSecret!,
                ["grant_type"] = "authorization_code",
                ["code"] = code,
                ["redirect_uri"] = settings.OAuthRedirectUri!
            }, cancellationToken);

            var now = DateTime.UtcNow;
            var account = await context.HotelPaymentAccounts.FirstOrDefaultAsync(a => a.HotelId == hotelId, cancellationToken);
            if (account is null)
            {
                account = new HotelPaymentAccount
                {
                    HotelId = hotelId,
                    PublicKey = token.PublicKey!,
                    AccessTokenEncrypted = Encrypt(token.AccessToken!),
                    RefreshTokenEncrypted = Encrypt(token.RefreshToken!),
                    ConnectedAtUtc = now
                };
                context.HotelPaymentAccounts.Add(account);
            }

            ApplyToken(account, token, now);
            account.ConnectedByUserId = userId;
            account.ConnectedAtUtc = now;

            await context.SaveChangesAsync(cancellationToken);

            logger.LogInformation(
                "Hotel {HotelId} connected Mercado Pago seller {MercadoPagoUserId} (live mode: {LiveMode}).",
                hotelId, account.MercadoPagoUserId, account.LiveMode);

            return hotelId;
        }

        public int? TryReadHotelIdFromState(string? state) =>
            state is not null && TryReadState(state, out int hotelId, out _, validateExpiry: false) ? hotelId : null;

        public async Task DisconnectAsync(int hotelId, CancellationToken cancellationToken = default)
        {
            var account = await context.HotelPaymentAccounts.FirstOrDefaultAsync(a => a.HotelId == hotelId, cancellationToken);
            if (account is null)
            {
                return;
            }

            context.HotelPaymentAccounts.Remove(account);
            await context.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Hotel {HotelId} disconnected Mercado Pago seller {MercadoPagoUserId}.", hotelId, account.MercadoPagoUserId);
        }

        // ---------------------------------------------------------- credentials

        public async Task<SellerCredentials?> GetCredentialsForHotelAsync(int hotelId, CancellationToken cancellationToken = default)
        {
            var account = await context.HotelPaymentAccounts.FirstOrDefaultAsync(a => a.HotelId == hotelId, cancellationToken);
            if (account is not null)
            {
                string? accessToken = await GetValidAccessTokenAsync(account, cancellationToken);
                if (accessToken is not null)
                {
                    return new SellerCredentials(hotelId, accessToken, account.PublicKey, IsPlatformAccount: false);
                }
            }

            return PlatformFallback(hotelId);
        }

        public async Task<SellerCredentials?> GetCredentialsForCollectorAsync(long mercadoPagoUserId, CancellationToken cancellationToken = default)
        {
            var account = await context.HotelPaymentAccounts.FirstOrDefaultAsync(a => a.MercadoPagoUserId == mercadoPagoUserId, cancellationToken);
            if (account is null)
            {
                return null;
            }

            string? accessToken = await GetValidAccessTokenAsync(account, cancellationToken);
            return accessToken is null ? null : new SellerCredentials(account.HotelId, accessToken, account.PublicKey, IsPlatformAccount: false);
        }

        public decimal? CalculateApplicationFee(decimal amount)
        {
            if (settings.MarketplaceFeePercent <= 0 || amount <= 0)
            {
                return null;
            }

            decimal fee = Math.Round(amount * settings.MarketplaceFeePercent / 100m, 2, MidpointRounding.AwayFromZero);
            return fee > 0 && fee < amount ? fee : null;
        }

        // -------------------------------------------------------------- helpers

        private SellerCredentials? PlatformFallback(int hotelId)
        {
            if (settings.UsePlatformAccountWhenHotelNotConnected
                && !string.IsNullOrWhiteSpace(settings.Token)
                && !string.IsNullOrWhiteSpace(settings.PublicKey))
            {
                logger.LogWarning("Hotel {HotelId} has no Mercado Pago account; using the platform account (fallback enabled).", hotelId);
                return new SellerCredentials(hotelId, settings.Token!, settings.PublicKey!, IsPlatformAccount: true);
            }

            return null;
        }

        private async Task<string?> GetValidAccessTokenAsync(HotelPaymentAccount account, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;
            if (account.AccessTokenExpiresAtUtc - now > RefreshWhenExpiringWithin)
            {
                return Decrypt(account.AccessTokenEncrypted);
            }

            try
            {
                OAuthTokenResponse token = await RequestTokenAsync(new Dictionary<string, string>
                {
                    ["client_id"] = settings.ClientId!,
                    ["client_secret"] = settings.ClientSecret!,
                    ["grant_type"] = "refresh_token",
                    ["refresh_token"] = Decrypt(account.RefreshTokenEncrypted)
                }, cancellationToken);

                ApplyToken(account, token, now);
                await context.SaveChangesAsync(cancellationToken);
                logger.LogInformation("Refreshed Mercado Pago token for hotel {HotelId}.", account.HotelId);
                return token.AccessToken;
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Could not refresh Mercado Pago token for hotel {HotelId}.", account.HotelId);
                // Still usable until it actually expires.
                return account.AccessTokenExpiresAtUtc > now ? Decrypt(account.AccessTokenEncrypted) : null;
            }
        }

        private void ApplyToken(HotelPaymentAccount account, OAuthTokenResponse token, DateTime now)
        {
            account.PublicKey = token.PublicKey!;
            account.AccessTokenEncrypted = Encrypt(token.AccessToken!);
            if (!string.IsNullOrWhiteSpace(token.RefreshToken))
            {
                account.RefreshTokenEncrypted = Encrypt(token.RefreshToken);
            }
            account.MercadoPagoUserId = token.UserId;
            account.LiveMode = token.LiveMode;
            account.AccessTokenExpiresAtUtc = now.AddSeconds(token.ExpiresIn > 0 ? token.ExpiresIn : 15_552_000);
            account.UpdatedAtUtc = now;
        }

        private async Task<OAuthTokenResponse> RequestTokenAsync(Dictionary<string, string> form, CancellationToken cancellationToken)
        {
            using var response = await httpClient.PostAsync(
                $"{settings.ApiBaseUrl.TrimEnd('/')}/oauth/token",
                new FormUrlEncodedContent(form),
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                // The body may echo request data; log only the status code.
                logger.LogError("Mercado Pago OAuth token request failed with status {StatusCode}.", (int)response.StatusCode);
                throw new InvalidOperationException("Mercado Pago rechazó la autorización. Volvé a intentarlo.");
            }

            var token = await response.Content.ReadFromJsonAsync<OAuthTokenResponse>(cancellationToken: cancellationToken);
            if (token is null
                || string.IsNullOrWhiteSpace(token.AccessToken)
                || string.IsNullOrWhiteSpace(token.PublicKey)
                || token.UserId <= 0)
            {
                throw new InvalidOperationException("Mercado Pago devolvió una respuesta incompleta.");
            }

            return token;
        }

        private async Task<bool> IsSuperAdminAsync(string userId, CancellationToken cancellationToken) =>
            await (from userRole in context.UserRoles
                   join role in context.Roles on userRole.RoleId equals role.Id
                   where userRole.UserId == userId && role.Name == "SuperAdmin"
                   select userRole).AnyAsync(cancellationToken);

        private void EnsureMarketplaceConfigured()
        {
            if (!settings.IsMarketplaceConfigured)
            {
                throw new InvalidOperationException("Mercado Pago Marketplace no está configurado en el servidor.");
            }
        }

        private static HotelPaymentAccountStatus ToStatus(HotelPaymentAccount account) =>
            new(account.HotelId, true, account.MercadoPagoUserId, account.LiveMode, account.ConnectedAtUtc);

        private static HotelPaymentAccountStatus NotConnected(int hotelId) =>
            new(hotelId, false, null, false, null);

        // ---------------------------------------------- state (HMAC-SHA256 signed)

        private string CreateState(int hotelId, string userId)
        {
            long expires = DateTimeOffset.UtcNow.Add(StateLifetime).ToUnixTimeSeconds();
            string nonce = Base64Url(RandomNumberGenerator.GetBytes(16));
            string payload = Base64Url(Encoding.UTF8.GetBytes($"{hotelId}|{userId}|{expires}|{nonce}"));
            return $"{payload}.{Sign(payload)}";
        }

        private bool TryReadState(string state, out int hotelId, out string userId, bool validateExpiry = true)
        {
            hotelId = 0;
            userId = string.Empty;

            string[] parts = state.Split('.');
            if (parts.Length != 2 || !CryptographicOperations.FixedTimeEquals(
                    Encoding.ASCII.GetBytes(Sign(parts[0])), Encoding.ASCII.GetBytes(parts[1])))
            {
                return false;
            }

            string[] fields;
            try
            {
                fields = Encoding.UTF8.GetString(FromBase64Url(parts[0])).Split('|');
            }
            catch (FormatException)
            {
                return false;
            }

            if (fields.Length != 4
                || !int.TryParse(fields[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out hotelId)
                || !long.TryParse(fields[2], NumberStyles.Integer, CultureInfo.InvariantCulture, out long expires))
            {
                return false;
            }

            userId = fields[1];
            return !validateExpiry || DateTimeOffset.UtcNow.ToUnixTimeSeconds() <= expires;
        }

        private string Sign(string payload)
        {
            byte[] key = HKDF.DeriveKey(HashAlgorithmName.SHA256, EncryptionKey, 32, info: Encoding.UTF8.GetBytes("mp-oauth-state"));
            return Base64Url(HMACSHA256.HashData(key, Encoding.UTF8.GetBytes(payload)));
        }

        // ------------------------------------------- token encryption (AES-256-GCM)

        private byte[] EncryptionKey
        {
            get
            {
                byte[] key;
                try
                {
                    key = Convert.FromBase64String(settings.TokenEncryptionKey ?? string.Empty);
                }
                catch (FormatException)
                {
                    throw new InvalidOperationException("MercadoPago:TokenEncryptionKey debe ser base64.");
                }

                return key.Length == 32
                    ? key
                    : throw new InvalidOperationException("MercadoPago:TokenEncryptionKey debe tener 32 bytes (base64).");
            }
        }

        private string Encrypt(string plaintext)
        {
            byte[] nonce = RandomNumberGenerator.GetBytes(AesGcm.NonceByteSizes.MaxSize);
            byte[] plain = Encoding.UTF8.GetBytes(plaintext);
            byte[] cipher = new byte[plain.Length];
            byte[] tag = new byte[AesGcm.TagByteSizes.MaxSize];

            using (var aes = new AesGcm(EncryptionKey, tag.Length))
            {
                aes.Encrypt(nonce, plain, cipher, tag);
            }

            return Convert.ToBase64String([.. nonce, .. tag, .. cipher]);
        }

        private string Decrypt(string encrypted)
        {
            byte[] data = Convert.FromBase64String(encrypted);
            int nonceSize = AesGcm.NonceByteSizes.MaxSize;
            int tagSize = AesGcm.TagByteSizes.MaxSize;

            byte[] nonce = data[..nonceSize];
            byte[] tag = data[nonceSize..(nonceSize + tagSize)];
            byte[] cipher = data[(nonceSize + tagSize)..];
            byte[] plain = new byte[cipher.Length];

            using (var aes = new AesGcm(EncryptionKey, tagSize))
            {
                aes.Decrypt(nonce, cipher, tag, plain);
            }

            return Encoding.UTF8.GetString(plain);
        }

        private static string Base64Url(byte[] bytes) =>
            Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');

        private static byte[] FromBase64Url(string value)
        {
            string base64 = value.Replace('-', '+').Replace('_', '/');
            return Convert.FromBase64String(base64.PadRight(base64.Length + (4 - base64.Length % 4) % 4, '='));
        }

        private sealed class OAuthTokenResponse
        {
            [JsonPropertyName("access_token")] public string? AccessToken { get; set; }
            [JsonPropertyName("refresh_token")] public string? RefreshToken { get; set; }
            [JsonPropertyName("public_key")] public string? PublicKey { get; set; }
            [JsonPropertyName("user_id")] public long UserId { get; set; }
            [JsonPropertyName("expires_in")] public long ExpiresIn { get; set; }
            [JsonPropertyName("live_mode")] public bool LiveMode { get; set; }
        }
    }
}
