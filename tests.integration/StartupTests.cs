using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using PokeShake.Api;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Integration.PokeShake.Api
{
    [Collection("Startup")]
    public class StartupTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly HttpClient _client;

        public StartupTests(WebApplicationFactory<Startup> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Startup_ShouldBootstrapSwaggerEndpoint()
        {
            using var response = await _client.GetAsync("/swagger");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Startup_ShouldBootstrapHealthcheckEndpoint()
        {
            using var response = await _client.GetAsync("/healthcheck");
            var content = await response.Content.ReadAsStringAsync();

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            content.Should().Be("Healthy");
        }

        [Fact]
        public async Task Startup_ShouldReturnNotFound_WhenANonExistantEndpointIsRequested()
        {
            using var response = await _client.GetAsync("/non-existant-endpoint");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
