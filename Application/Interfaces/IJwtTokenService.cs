using Infrastructure.Models;

namespace Application.Interfaces
{
    public interface IJwtTokenService
    {
        Task<string> CreateTokenAsync(ApplicationUser user);
    }
}
