using AuthServiceApp.API;
using AuthServiceApp.API.Data;
using AuthServiceApp.API.Entities;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace AuthServiceApp.Tests.ControllerTests
{
    [Trait("Category", "Integration")]
    public class UserControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> factory;
        private readonly ApplicationDbContext context;

        public UserControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            this.factory = factory;
            context = factory.Services!.CreateScope()!.ServiceProvider!.GetService<ApplicationDbContext>()!;
        }

        [Fact]
        public async Task GetUsers_Ok_Test()
        {
            var client = factory.CreateClient();
            var response = await client.GetAsync("/api/User");

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task GetUsers_AfterAddNewUser_Test()
        {
            var client = factory.CreateClient();
            var response1 = await client.GetAsync("/api/User");
            response1.EnsureSuccessStatusCode();

            var usersListBefore = JsonConvert.DeserializeObject<IEnumerable<User>>(await response1.Content.ReadAsStringAsync())!;
            
            var newUser = new User
            {
                Id = Guid.NewGuid(),
                Username = $"user_{Guid.NewGuid()}",
                PasswordHash = "user"
            };

            await context.Users.AddAsync(newUser);
            await context.SaveChangesAsync();

            var response2 = await client.GetAsync("/api/User");
            response2.EnsureSuccessStatusCode();

            var usersListAfter = JsonConvert.DeserializeObject<IEnumerable<User>>(await response2.Content.ReadAsStringAsync())!;

            Assert.Equal(1, usersListAfter.Count() - usersListBefore.Count());
        }
    }
        
}
