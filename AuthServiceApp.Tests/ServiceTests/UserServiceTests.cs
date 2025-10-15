using AuthServiceApp.API;
using AuthServiceApp.API.Data;
using AuthServiceApp.API.Entities;
using AuthServiceApp.API.Interfaces.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AuthServiceApp.Tests.ServicesTests
{
    public class UserServiceTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> factory;
        private readonly ApplicationDbContext context;
        private readonly IUserService userService;

        public UserServiceTests(CustomWebApplicationFactory<Program> factory)
        {
            this.factory = factory;

            context = factory.Services!.CreateScope()!.ServiceProvider!.GetRequiredService<ApplicationDbContext>();
            userService = factory.Services!.CreateScope()!.ServiceProvider!.GetRequiredService<IUserService>()!;
        }

        [Fact]
        public async Task UsersCount_Test()
        {
            var usersCountFromDb = await context.Users.CountAsync();
            var users = await userService.GetUsers();

            Assert.Equal(usersCountFromDb, users.Count());
        }

        [Fact]
        public async Task UsersCount_AfterAddOne_Test()
        {
            var usersCountFromDbBefore = await context.Users.CountAsync();
            var usersBefore = await userService.GetUsers();
            
            Assert.Equal(usersCountFromDbBefore, usersBefore.Count());

            var newUser = new User
            {
                Id = Guid.NewGuid(),
                Username = $"user_{Guid.NewGuid()}",
                PasswordHash = "user"
            };

            await context.Users.AddAsync(newUser);
            await context.SaveChangesAsync();

            var usersCountFromDbAfter = await context.Users.CountAsync();
            var usersAfter = await userService.GetUsers();
            
            Assert.Equal(usersCountFromDbAfter, usersAfter.Count());
        }
    }
}
