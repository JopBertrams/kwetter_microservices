using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using HonkService.Domain.Entities;
using HonkService.Infrastructure;
using System.Net.Http.Headers;
using NUnit.Framework;
using System.Linq;
using System.Net;
using System.IO;
using Auth0.AuthenticationApi;
using Auth0.AuthenticationApi.Models;
using Newtonsoft.Json;
using System.Text;
using HonkService.Api.DTO;

namespace HonkService.Tests
{
    public class IntegrationTests
    {
        private readonly HttpClient httpClient;
        private readonly string clientId;
        private readonly string clientSecret;
        private readonly string domain;
        private readonly string audience;

        public IntegrationTests()
        {
            var application = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        var mySqlDbConnection = services.SingleOrDefault(
                            d => d.ServiceType ==
                                typeof(DbContextOptions<HonkContext>));

                        services.Remove(mySqlDbConnection!);

                        services.AddDbContext<HonkContext>(options =>
                        {
                            options.UseInMemoryDatabase("InMemoryDbForTesting");
                        });
                    });
                });

            httpClient = application.CreateClient();

            AddHonksToDatabase();

            clientId = Environment.GetEnvironmentVariable("CLIENT_ID");
            clientSecret = Environment.GetEnvironmentVariable("CLIENT_SECRET");
            domain = Environment.GetEnvironmentVariable("DOMAIN");
            audience = Environment.GetEnvironmentVariable("AUDIENCE");
        }

        private void AddHonksToDatabase()
        {
            var _contextOptions = new DbContextOptionsBuilder<HonkContext>()
                .UseInMemoryDatabase("InMemoryDbForTesting")
                .Options;

            using var context = new HonkContext(_contextOptions);

            context.Database.EnsureCreated();

            context.AddRange(
                new HonkEntity
                {
                    Id = Guid.Parse("3743d0a5-195a-47b6-a81a-50bc7a64c9d8"),
                    UserId = "UniqueTestUserId",
                    Message = "TestHonk1",
                    CreatedAt = DateTime.UtcNow
                },
                new HonkEntity
                {
                    Id = Guid.Parse("86c2553c-a45f-4910-8d0d-11bf24ea93aa"),
                    UserId = "UniqueTestUserId",
                    Message = "TestHonk2",
                    CreatedAt = DateTime.UtcNow
                }
            );

            context.SaveChanges();
        }

        private bool HonkExistsInDatabase(string message)
        {
            var _contextOptions = new DbContextOptionsBuilder<HonkContext>()
                .UseInMemoryDatabase("InMemoryDbForTesting")
                .Options;

            using var context = new HonkContext(_contextOptions);

            return context.Honks.Any(h => h.Message == message);
        }

        private async Task<string> GetAccessToken()
        {
            var auth0Client = new AuthenticationApiClient(domain);
            var tokenRequest = new ClientCredentialsTokenRequest()
            {
                ClientId = clientId,
                ClientSecret = clientSecret,
                Audience = audience
            };
            var tokenResponse = await auth0Client.GetTokenAsync(tokenRequest);

            return tokenResponse.AccessToken;
        }

        [Test]
        public async Task GetHonkById_NoAuthenticationToken()
        {
            // Act
            var request = new HttpRequestMessage(HttpMethod.Get, "api/honk/ae591b0e-484e-48c4-932c-d954e2b99d56");
            var response = await httpClient.SendAsync(request);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public async Task GetHonkById_InvalidAuthenticationToken()
        {
            // Act
            var request = new HttpRequestMessage(HttpMethod.Get, "api/honk/ae591b0e-484e-48c4-932c-d954e2b99d56");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", "InvalidAuth0Token");
            var response = await httpClient.SendAsync(request);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
            Assert.That(response.Headers.GetValues("WWW-Authenticate").First(), Is.EqualTo($"Bearer error=\"invalid_token\""));
        }

        [Test]
        public async Task GetHonkById_HonkDoesNotExist()
        {
            // Arrange
            var accessToken = await GetAccessToken();

            // Act
            var request = new HttpRequestMessage(HttpMethod.Get, "api/honk/ae591b0e-484e-48c4-932c-d954e2b99d56");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await httpClient.SendAsync(request);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        public async Task GetHonkById_Success()
        {
            // Arrange
            var accessToken = await GetAccessToken();

            // Act
            var request = new HttpRequestMessage(HttpMethod.Get, "api/honk/3743d0a5-195a-47b6-a81a-50bc7a64c9d8");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await httpClient.SendAsync(request);
            var responseBody = await response.Content.ReadAsStreamAsync();
            var streamReader = new StreamReader(responseBody);
            var jsonReader = new JsonTextReader(streamReader);
            JsonSerializer serializer = new JsonSerializer();
            var honk = serializer.Deserialize<HonkEntity>(jsonReader);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(honk.Id, Is.EqualTo(Guid.Parse("3743d0a5-195a-47b6-a81a-50bc7a64c9d8")));
            Assert.That(honk.Message, Is.EqualTo("TestHonk1"));
        }

        [Test]
        public async Task PostHonk_UserIdMissing()
        {
            // Arrange
            var accessToken = await GetAccessToken();

            // Act
            var request = new HttpRequestMessage(HttpMethod.Post, "api/honk");
            request.Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(new
            {
                message = "The username is missing"
            }), Encoding.UTF8, "application/json");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await httpClient.SendAsync(request);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        public async Task PostHonk_MessageMissing()
        {
            // Arrange
            var accessToken = await GetAccessToken();

            // Act
            var request = new HttpRequestMessage(HttpMethod.Post, "api/honk");
            request.Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(new
            {
                UserId = "UniqueUserId"
            }), Encoding.UTF8, "application/json");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await httpClient.SendAsync(request);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        public async Task PostHonk_Success()
        {
            // Arrange
            var accessToken = await GetAccessToken();

            // Act
            var request = new HttpRequestMessage(HttpMethod.Post, "api/honk");
            request.Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(new
            {
                UserId = "UniqueUserId",
                Message = "Test message..."
            }), Encoding.UTF8, "application/json");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await httpClient.SendAsync(request);
            var responseBody = await response.Content.ReadAsStreamAsync();
            var streamReader = new StreamReader(responseBody);
            var jsonReader = new JsonTextReader(streamReader);
            JsonSerializer serializer = new JsonSerializer();
            var honk = serializer.Deserialize<HonkResponseDTO>(jsonReader);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.IsInstanceOf<Guid>(honk.Id);
            Assert.That(honk.UserId, Is.EqualTo("UniqueUserId"));
            Assert.That(honk.Message, Is.EqualTo("Test message..."));
            Assert.That(HonkExistsInDatabase("Test message..."), Is.True);
        }

        [Test]
        public async Task DeleteHonk_HonkDoesNotExist()
        {
            // Arrange
            var accessToken = await GetAccessToken();

            // Act
            var request = new HttpRequestMessage(HttpMethod.Delete, "api/honk/ae591b0e-484e-48c4-932c-d954e2b99d56");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await httpClient.SendAsync(request);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        public async Task DeleteHonk_Success()
        {
            // Arrange
            var accessToken = await GetAccessToken();

            // Act
            var request = new HttpRequestMessage(HttpMethod.Delete, "api/honk/86c2553c-a45f-4910-8d0d-11bf24ea93aa");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await httpClient.SendAsync(request);
            var responseBody = await response.Content.ReadAsStreamAsync();
            var streamReader = new StreamReader(responseBody);
            var jsonReader = new JsonTextReader(streamReader);
            JsonSerializer serializer = new JsonSerializer();
            var honk = serializer.Deserialize<HonkResponseDTO>(jsonReader);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.IsInstanceOf<Guid>(honk.Id);
            Assert.That(HonkExistsInDatabase("TestHonk2"), Is.False);
        }
    }
}
