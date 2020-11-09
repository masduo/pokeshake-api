using FluentAssertions;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using PokeShake.Api;
using PokeShake.Api.Features.Pokemon;
using PokeShake.Api.Services.FunTranslations.Interfaces;
using PokeShake.Api.Services.Pokemon.Interfaces;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Text.Json;
using System.Threading.Tasks;
using Tests.Integration.Fakes;
using Xunit;

namespace Tests.Integration.Features.Pokemon
{
    [Collection("Controllers")]
    public class PokemonControllerTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private const string PokemonV1ResourceLocation = "/v1/pokemon";
        private const string Charizard = "charizard";

        private static string _charizardUri = $"{PokemonV1ResourceLocation}/{Charizard}";

        private readonly WebApplicationFactory<Startup> _factory;
        private readonly HttpClient _client;

        public PokemonControllerTests(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;

            _client = factory.WithWebHostBuilder(webHostBuilder =>
                webHostBuilder.ConfigureTestServices(services =>
                    // fake the service dependencies of the controller
                    services
                        .AddTransient<IPokeService, FakePokeService>()
                        .AddTransient<IFunTranslationsService, FakeFunTranslationsService>()))
                .CreateClient();
        }

        [Fact]
        public async Task Translate_ShouldReturnNotFound_WhenNameIsNotProvided()
        {
            using var response = await _client.GetAsync($"{PokemonV1ResourceLocation}/{string.Empty}");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Translate_ShouldReturnBadRequest_WhenNameIsWhiteSpace()
        {
            const string whitespace = "   ";
            var encodedUri = UriHelper.Encode(new Uri($"{PokemonV1ResourceLocation}/{whitespace}", UriKind.Relative));
            using var response = await _client.GetAsync(encodedUri);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Translate_ShouldReturnBadRequest_WhenVersion2IsRequested()
        {
            using var response = await _client.GetAsync($"/v2/pokemon/{Charizard}");
            var content = await response.Content.ReadAsStringAsync();

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            content.Should().Contain("UnsupportedApiVersion");
        }

        [Fact]
        public async Task Translate_ShouldReturnNotFound_WhenNoFlavorTextIsRetruned()
        {
            // arrange
            var client = _factory.WithWebHostBuilder(webHostBuilder =>
                webHostBuilder.ConfigureTestServices(services =>
                    {
                        // override PokeService response
                        services.AddTransient<IPokeService>(implementationFactory =>
                            new FakePokeService(fakeFlavor: default(string)));

                        services.AddTransient<IFunTranslationsService, FakeFunTranslationsService>();
                    }))
                    .CreateClient();

            // act
            using var response = await client.GetAsync(_charizardUri);
            var content = await response.Content.ReadAsStringAsync();

            // assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            content.Should().Contain($"No Pokemon species found for '{Charizard}'");
        }

        [Fact]
        public async Task Translate_ShouldReturnNotFound_WhenNoTranslationIsRetruned()
        {
            // arrange
            var client = _factory.WithWebHostBuilder(webHostBuilder =>
                webHostBuilder.ConfigureTestServices(services =>
                    {
                        services.AddTransient<IPokeService, FakePokeService>();

                        // override FunTranslationsService response
                        services.AddTransient<IFunTranslationsService>(implementationFactory =>
                            new FakeFunTranslationsService(fakeTranslation: default(string)));
                    }))
                    .CreateClient();

            // act
            using var response = await client.GetAsync(_charizardUri);
            var content = await response.Content.ReadAsStringAsync();

            // assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            content.Should().Contain($"Translation failed for '{Charizard}', or rate limit is reached");
        }

        [Fact]
        public async Task Translate_ShouldReturnOKStatus()
        {
            using var response = await _client.GetAsync(_charizardUri);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Translate_ShouldReturnJsonContent()
        {
            using var response = await _client.GetAsync(_charizardUri);

            response.Content.Headers.GetValues("Content-Type")
                .Should().Contain($"{MediaTypeNames.Application.Json}; charset=utf-8");
        }

        [Fact]
        public async Task Translate_ShouldReturnV1ForSupportedVersionHeader()
        {
            using var response = await _client.GetAsync(_charizardUri);

            response.Headers.GetValues("api-supported-versions").Should().Contain("1.0");
        }

        [Fact]
        public async Task Translate_ShouldReturnResponseWithPokemonResponseSchema()
        {
            using var response = await _client.GetAsync(_charizardUri);

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var translation = await JsonSerializer.DeserializeAsync<PokemonResponse>(
                await response.Content.ReadAsStreamAsync(), options);

            translation.Name.Should().Be(Charizard);
            translation.Description.Should().NotBeEmpty();
        }
    }
}
