using System.Security.Claims;

namespace AuthServiceApp.API.Interfaces.Services
{
    public interface IJwtService
    {
        public int RefreshTokenValidityDays { get; }

        public string GenerateAccessToken(List<Claim> claims);
        public string GenerateRefreshToken();
        public IEnumerable<Claim> GetTokenClaims(string token);
    }
}
