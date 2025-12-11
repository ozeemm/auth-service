using AuthServiceApp.API;
using AuthServiceApp.API.Data;
using AuthServiceApp.API.Entities;
using AuthServiceApp.API.Models.Dtos;
using AuthServiceApp.API.Models.Requets;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http.Json;

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

            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = "admin",
                PasswordHash = "AQAAAAIAAYagAAAAEP5VRivxbsgYOT6QL0xsr3oAx1rHkZvjsGmQNwDtui/hFjrt0p5Y7UCWi+3vQSZaFg==" // admin
            };
            context.Users.Add(user);
            context.SaveChanges();
        }

        [Fact]
        public async Task GetUsers_Ok_Test()
        {
            var client = factory.CreateClient();

            var tokens = await GetTokens(client);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokens!.AccessToken);

            var response = await client.GetAsync("/api/User");

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task GetUsers_AfterAddNewUser_Test()
        {
            var client = factory.CreateClient();

            var tokens = await GetTokens(client);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokens!.AccessToken);

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

        private async Task<TokenDto> GetTokens(HttpClient client)
        {
            var request = new UserRequest
            {
                Username = $"user_{Guid.NewGuid()}",
                Password = "user"
            };

            var registerResponse = await client.PostAsJsonAsync("/api/auth/register", request);
            registerResponse.EnsureSuccessStatusCode();

            var loginResponse = await client.PostAsJsonAsync("/api/auth/login", request);
            loginResponse.EnsureSuccessStatusCode();

            var tokens = JsonConvert.DeserializeObject<TokenDto>(await loginResponse.Content.ReadAsStringAsync())!;

            return tokens;
        }
    }

}
