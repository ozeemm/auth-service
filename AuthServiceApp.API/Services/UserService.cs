using AuthServiceApp.API.Data;
using AuthServiceApp.API.Entities;
using AuthServiceApp.API.Interfaces.Services;
using Microsoft.EntityFrameworkCore;

namespace AuthServiceApp.API.Services
{
    public class UserService(
        ApplicationDbContext context) : IUserService
    {
        public async Task<IEnumerable<User>> GetUsers()
        {
            return await context.Users.ToListAsync();
        }
    }
}
