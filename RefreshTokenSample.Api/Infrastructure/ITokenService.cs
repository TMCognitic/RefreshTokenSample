using RefreshTokenSample.Api.Models.Entities;

namespace RefreshTokenSample.Api.Infrastructure
{
    public interface ITokenService
    {
        string GenerateRefreshToken();
        void GenerateToken(User user);
        TokenPair RefreshTokenPair(string oldToken);
    }
}