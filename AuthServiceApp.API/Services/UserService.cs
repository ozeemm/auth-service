using AuthServiceApp.API.Data;
using AuthServiceApp.API.Interfaces.Services;
using AuthServiceApp.API.Models.Dtos;
using Microsoft.EntityFrameworkCore;

namespace AuthServiceApp.API.Services
{
    public class UserService(
        ApplicationDbContext context) : IUserService
    {
        public async Task<IEnumerable<UserDto>> GetUsers()
        {
            return await context.Users
                .Select(u => new UserDto { Id = u.Id, Username = u.Username })
                .ToListAsync();
        }
    }
}
