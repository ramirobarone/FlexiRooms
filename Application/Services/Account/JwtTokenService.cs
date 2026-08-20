using Application.Interfaces;
using Application.Models.Options;
using Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Application.Services.Account
{
    public class JwtTokenService(UserManager<ApplicationUser> userManager, IOptions<JwtOptions> jwtOptions) : IJwtTokenService
    {
        public async Task<string> CreateTokenAsync(ApplicationUser user)
        {
            ArgumentNullException.ThrowIfNull(user);

            JwtOptions options = jwtOptions.Value;
            string key = options.Key ?? throw new InvalidOperationException("JWT signing key is required.");
            IList<string> roles = await userManager.GetRolesAsync(user);

            List<Claim> claims =
            [
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Sub, user.Id),
                new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                new(ClaimTypes.NameIdentifier, user.Id),
                new(ClaimTypes.Name, user.UserName ?? user.Email ?? string.Empty),
                new("full_name", $"{user.Name} {user.LastName}".Trim()),
                new("user_guid", user.UserGuid.ToString()),
            ];

            if (user.ManagedHotelId.HasValue)
            {
                claims.Add(new Claim("managed_hotel_id", user.ManagedHotelId.Value.ToString()));
            }

            if (!string.IsNullOrWhiteSpace(user.OwnedHotelIds))
            {
                claims.Add(new Claim("owned_hotel_ids", user.OwnedHotelIds));
            }

            foreach (string role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            JwtSecurityToken token = new(
                issuer: options.Authority,
                audience: options.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(options.TokenDuration),
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                    SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
