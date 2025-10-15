using AuthServiceApp.API.Data;
using AuthServiceApp.API.Entities;
using AuthServiceApp.API.Interfaces.Services;
using AuthServiceApp.API.Models.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AuthServiceApp.API.Services
{
    public class AuthService(
        ApplicationDbContext context, IJwtService jwtService) : IAuthService
    {
        private List<Claim> GetClaims(User user, string jti)
        {
            return new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, jti)
            };
        }

        public async Task<User?> RegisterAsync(string username, string password)
        {
            var isUserExist = await context.Users
                .Where(u => u.Username == username)
                .AnyAsync();

            if(isUserExist)
                return null;

            var user = new User();

            var hashedPassword = new PasswordHasher<User>()
                .HashPassword(user, password);

            user.Id = Guid.NewGuid();
            user.Username = username;
            user.PasswordHash = hashedPassword;

            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            return user;
        }

        public async Task<TokenDto?> LoginAsync(string username, string password)
        {
            var user = await context.Users
                .Where(u => u.Username == username)
                .FirstOrDefaultAsync();

            if (user is null)
                return null;

            if (new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, password) == PasswordVerificationResult.Failed)
                return null;

            var jti = Guid.NewGuid().ToString();

            var tokenDto = new TokenDto {
                AccessToken = jwtService.GenerateAccessToken(GetClaims(user, jti)),
                RefreshToken = jwtService.GenerateRefreshToken()
            };

            user.RefreshToken = tokenDto.RefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(jwtService.RefreshTokenValidityDays);
            user.Jti = jti;
            await context.SaveChangesAsync();

            return tokenDto;
        }

        public async Task<TokenDto?> RefreshTokensAsync(string accessToken, string refreshToken)
        {
            var jtiFromClaims = jwtService.GetTokenClaims(accessToken)
                .FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Jti)?
                .Value;

            if (jtiFromClaims is null)
                return null;

            var userId = jwtService.GetTokenClaims(accessToken)
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?
                .Value;

            if (userId is null)
                return null;

            var user = await context.Users.FindAsync(Guid.Parse(userId));
            
            if (user is null)
                return null;

            if (user.Jti != null && user.Jti != jtiFromClaims)
                return null;

            if (user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                return null;

            var jti = Guid.NewGuid().ToString();

            var tokenDto = new TokenDto
            {
                AccessToken = jwtService.GenerateAccessToken(GetClaims(user, jti)),
                RefreshToken = jwtService.GenerateRefreshToken()
            };

            user.RefreshToken = tokenDto.RefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(jwtService.RefreshTokenValidityDays);
            user.Jti = jti;
            await context.SaveChangesAsync();

            return tokenDto;
        }

        public async Task<bool> LogoutAsync(string accessToken)
        {
            var userId = jwtService.GetTokenClaims(accessToken)
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?
                .Value;

            var user = await context.Users.FindAsync(Guid.Parse(userId));

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;
            user.Jti = null;

            await context.SaveChangesAsync();
            return true;
        }
    }
}
