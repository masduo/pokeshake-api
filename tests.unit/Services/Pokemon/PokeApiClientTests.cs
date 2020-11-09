using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using PokeShake.Api.Services.Pokemon;
using PokeShake.Api.Services.Pokemon.Models;
using PokeShake.Api.Services.Pokemon.Settings;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Tests.Unit.Fakes;
using Xunit;

namespace Tests.Unit.Services.Pokemon
{
    [Collection("Services")]
    public partial class PokeApiClientTests
    {
        private PokeApiOptions _pokeApiOptions = new PokeApiOptions { BaseUri = "http://mock.url" };

        [Fact]
        public async Task GetSpecies_ShouldReturnDefault_WhenSpeciesIsNotFound()
        {
            // arrange
            var loggerMock = new Mock<ILogger<PokeApiClient>>();

            var pokeApiOptions = Options.Create(_pokeApiOptions);

            var httpResponseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NotFound
            };
            var httpClient = new HttpClient(new FakeHttpMessageHandler(httpResponseMessage));

            var pokeApiClient = new PokeApiClient(loggerMock.Object, pokeApiOptions, httpClient);

            // act
            const string SpeciesName = "non-existant-species";
            var species = await pokeApiClient.GetSpecies(SpeciesName);

            // assert
            species.Should().Be(default(PokemonSpecies));

            loggerMock.Verify(LogLevel.Information, $"No Pokemon species found for '{SpeciesName}'");
        }

        [Fact]
        public async Task GetSpecies_ShouldDeserialisePokeApiResponseToPokemonSpeciesModel()
        {
            // arrange
            var pokeApiOptions = Options.Create(_pokeApiOptions);

            var content = new
            {
                flavor_text_entries = new[]
             {
                    new
                    {
                        flavor_text = "It uses its wings\nto fly high. The\ntemperature of its\ffire increases as\nit gains exper­\nience in battle.",
                        language = new { name = "en" },
                        version = new { name = "crystal" }
                    },
                    new
                    {
                        flavor_text = "CHARIZARD flies around the sky in search of powerful opponents",
                        language = new { name = "en" },
                        version = new { name = "ruby" }
                    }
                }
            };
            var httpResponseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(content))
            };
            var httpClient = new HttpClient(new FakeHttpMessageHandler(httpResponseMessage));

            var pokeApiClient = new PokeApiClient(logger: null, pokeApiOptions, httpClient);

            // act
            var species = await pokeApiClient.GetSpecies("charizard");

            // assert
            species.FlavorTextEntries.Count().Should().Be(content.flavor_text_entries.Count());

            species.FlavorTextEntries.First().FlavorText.Should().Be(content.flavor_text_entries.First().flavor_text);
            species.FlavorTextEntries.First().Language.Name.Should().Be(content.flavor_text_entries.First().language.name);
            species.FlavorTextEntries.First().Version.Name.Should().Be(content.flavor_text_entries.First().version.name);

            species.FlavorTextEntries.Last().FlavorText.Should().Be(content.flavor_text_entries.Last().flavor_text);
            species.FlavorTextEntries.Last().Language.Name.Should().Be(content.flavor_text_entries.Last().language.name);
            species.FlavorTextEntries.Last().Version.Name.Should().Be(content.flavor_text_entries.Last().version.name);
        }
    }
}
