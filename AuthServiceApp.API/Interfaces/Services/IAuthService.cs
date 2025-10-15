using AuthServiceApp.API.Entities;
using AuthServiceApp.API.Models.Dtos;
namespace AuthServiceApp.API.Interfaces.Services
{
    public interface IAuthService
    {
        public Task<User?> RegisterAsync(string username, string password);
        public Task<TokenDto?> LoginAsync(string username, string password);
        public Task<TokenDto?> RefreshTokensAsync(string accessToken, string refreshToken);
        public Task<bool> LogoutAsync(string accessToken);
    }
}
