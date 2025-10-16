using AuthServiceApp.API;
using AuthServiceApp.API.Data;
using AuthServiceApp.API.Models.Dtos;
using AuthServiceApp.API.Models.Requets;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace AuthServiceApp.Tests.ControllerTests
{
    [Trait("Category", "Integration")]
    public class AuthControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> factory;
        private readonly ApplicationDbContext context;

        public AuthControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            this.factory = factory;
            context = factory.Services!.CreateScope()!.ServiceProvider!.GetService<ApplicationDbContext>()!;
        }

        [Fact]
        public async Task Register_OneUser_Test()
        {
            var client = factory.CreateClient();

            var request = new UserRequest
            {
                Username = $"user_{Guid.NewGuid()}",
                Password = "user"
            };

            var response = await client.PostAsJsonAsync("/api/auth/register", request);

            response.EnsureSuccessStatusCode();

            var user = await context.Users.Where(u => u.Username == request.Username).FirstOrDefaultAsync();
            Assert.NotNull(user);
        }

        [Fact]
        public async Task Register_TwoUsers_SameUsername_Test()
        {
            var client = factory.CreateClient();

            var request = new UserRequest
            {
                Username = $"user_{Guid.NewGuid()}",
                Password = "user"
            };

            var response1 = await client.PostAsJsonAsync("/api/auth/register", request);
            response1.EnsureSuccessStatusCode();

            var response2 = await client.PostAsJsonAsync("/api/auth/register", request);
            Assert.Equal(HttpStatusCode.BadRequest, response2.StatusCode);

            var userCount = await context.Users.Where(u => u.Username == request.Username).CountAsync();
            Assert.Equal(1, userCount);
        }

        [Fact]
        public async Task Login_Test()
        {
            var client = factory.CreateClient();

            var request = new UserRequest
            {
                Username = $"user_{Guid.NewGuid()}",
                Password = "user"
            };

            var registerResponse = await client.PostAsJsonAsync("/api/auth/register", request);
            registerResponse.EnsureSuccessStatusCode();

            var loginResponse = await client.PostAsJsonAsync("/api/auth/login", request);
            loginResponse.EnsureSuccessStatusCode();

            var tokens = JsonConvert.DeserializeObject<TokenDto>(await loginResponse.Content.ReadAsStringAsync());

            Assert.NotNull(tokens);
            Assert.False(string.IsNullOrEmpty(tokens.AccessToken));
            Assert.False(string.IsNullOrEmpty(tokens.RefreshToken));
        }

        [Fact]
        public async Task AuthOnlyRequest_Authorized_Test()
        {
            var client = factory.CreateClient();

            var request = new UserRequest
            {
                Username = $"user_{Guid.NewGuid()}",
                Password = "user"
            };

            var registerResponse = await client.PostAsJsonAsync("/api/auth/register", request);
            registerResponse.EnsureSuccessStatusCode();

            var loginResponse = await client.PostAsJsonAsync("/api/auth/login", request);
            loginResponse.EnsureSuccessStatusCode();

            var tokens = JsonConvert.DeserializeObject<TokenDto>(await loginResponse.Content.ReadAsStringAsync());
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokens!.AccessToken);

            var authOnlyResponse = await client.GetAsync("/api/examples/authOnly");
            authOnlyResponse.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task AuthOnlyRequest_NotAuthorized_Test()
        {
            var client = factory.CreateClient();

            var authOnlyResponse = await client.GetAsync("/api/examples/authOnly");
            Assert.Equal(HttpStatusCode.Unauthorized, authOnlyResponse.StatusCode);
        }

        [Fact]
        public async Task RefreshToken_Test()
        {
            var client = factory.CreateClient();

            var request = new UserRequest
            {
                Username = $"user_{Guid.NewGuid()}",
                Password = "user"
            };

            var registerResponse = await client.PostAsJsonAsync("/api/auth/register", request);
            registerResponse.EnsureSuccessStatusCode();

            var loginResponse = await client.PostAsJsonAsync("/api/auth/login", request);
            loginResponse.EnsureSuccessStatusCode();

            var tokens = JsonConvert.DeserializeObject<TokenDto>(await loginResponse.Content.ReadAsStringAsync());

            var refreshResponse = await client.PostAsJsonAsync("/api/auth/refresh", new RefreshTokensRequest 
            {
                AccessToken = tokens!.AccessToken,
                RefreshToken = tokens.RefreshToken
            });

            refreshResponse.EnsureSuccessStatusCode();
            var newTokens = JsonConvert.DeserializeObject<TokenDto>(await refreshResponse.Content.ReadAsStringAsync());

            Assert.NotNull(newTokens);
            Assert.False(string.IsNullOrEmpty(newTokens.AccessToken));
            Assert.False(string.IsNullOrEmpty(newTokens.RefreshToken));

            Assert.NotEqual(tokens.AccessToken, newTokens.AccessToken);
            Assert.NotEqual(tokens.RefreshToken, newTokens.RefreshToken);
        }

        [Fact]
        public async Task Logout_Test()
        {
            var client = factory.CreateClient();

            var request = new UserRequest
            {
                Username = $"user_{Guid.NewGuid()}",
                Password = "user"
            };

            var registerResponse = await client.PostAsJsonAsync("/api/auth/register", request);
            registerResponse.EnsureSuccessStatusCode();

            var loginResponse = await client.PostAsJsonAsync("/api/auth/login", request);
            loginResponse.EnsureSuccessStatusCode();

            var tokens = JsonConvert.DeserializeObject<TokenDto>(await loginResponse.Content.ReadAsStringAsync());
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokens!.AccessToken);

            var authOnlyAuthorizedResponse = await client.GetAsync("/api/examples/authOnly");
            authOnlyAuthorizedResponse.EnsureSuccessStatusCode();

            var logoutResponse = await client.PostAsync("/api/auth/logout", null);
            logoutResponse.EnsureSuccessStatusCode();

            var authOnlyUnauthorizedResponse = await client.GetAsync("/api/examples/authOnly");
            Assert.Equal(HttpStatusCode.Unauthorized, authOnlyUnauthorizedResponse.StatusCode);
        }
    }
}
