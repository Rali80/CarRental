using CarRental.API.Models;

namespace CarRental.API.Services
{
    public interface ITokenService
    {
        string GenerateAccessToken(ApplicationUser user);
        string GenerateRefreshToken();
        Task<RefreshToken> CreateRefreshToken(string userId);
        Task<RefreshToken?> ValidateRefreshToken(string token);
        Task RevokeRefreshToken(string token);
    }
}
