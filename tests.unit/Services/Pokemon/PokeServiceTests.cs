using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using PokeShake.Api.Services.Pokemon;
using PokeShake.Api.Services.Pokemon.Interfaces;
using PokeShake.Api.Services.Pokemon.Models;
using PokeShake.Api.Services.Pokemon.Settings;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Unit.Services.Pokemon
{
    [Collection("Services")]
    public class PokeServiceTests
    {
        private const string Charizard = "Charizard";

        private IOptions<PokeApiOptions> _pokeApiOptions = Options.Create(new PokeApiOptions
        {
            GameVersion = "ruby",
            Language = "en"
        });

        private FlavorTextEntry ValidFlavorTextEntry
        {
            get => new FlavorTextEntry
            {
                FlavorText = "ValidFlavorTextEntry",
                Version = new NamedApiResource
                {
                    Name = "ruby"
                },
                Language = new NamedApiResource
                {
                    Name = "en"
                }
            };
        }

        [Fact]
        public async Task GetFlavorText_ShouldReturnDefault_WhenGetSpeciesReturnsDefault()
        {
            // arrange
            var mockedSpecies = default(PokemonSpecies);
            var pokeApiClientMock = new Mock<IPokeApiClient>();
            pokeApiClientMock.Setup(m => m.GetSpecies(It.IsAny<string>())).ReturnsAsync(mockedSpecies);

            var pokeService = new PokeService(pokeApiClientMock.Object, _pokeApiOptions);

            // act
            var flavorText = await pokeService.GetFlavorText(Charizard);

            // assert
            flavorText.Should().Be(default(string));
        }

        [Fact]
        public void GetFlavorText_ShouldThrowArgumentNullException_WhenFlavorTextEntriesIsEmpty()
        {
            // arrange
            var mockedSpecies = new PokemonSpecies();
            var pokeApiClientMock = new Mock<IPokeApiClient>();
            pokeApiClientMock.Setup(m => m.GetSpecies(It.IsAny<string>())).ReturnsAsync(mockedSpecies);

            var pokeService = new PokeService(pokeApiClientMock.Object, _pokeApiOptions);

            Func<Task<string>> getFlavorTextDelegate =
                async () => await pokeService.GetFlavorText(Charizard);

            // act & assert
            getFlavorTextDelegate.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public async Task GetFlavorText_ShouldReturnDefault_WhenFlavorTextEntriesHasNoEnglishLanguageEntry()
        {
            // arrange
            var notEnglish = ValidFlavorTextEntry;
            notEnglish.Language.Name = "not-english";
            var mockedSpecies = new PokemonSpecies
            {
                FlavorTextEntries = new[] { notEnglish }
            };

            var pokeApiClientMock = new Mock<IPokeApiClient>();
            pokeApiClientMock.Setup(m => m.GetSpecies(It.IsAny<string>())).ReturnsAsync(mockedSpecies);

            var pokeService = new PokeService(pokeApiClientMock.Object, _pokeApiOptions);

            // act
            var flavorText = await pokeService.GetFlavorText(Charizard);

            // assert
            flavorText.Should().Be(default);
        }

        [Fact]
        public async Task GetFlavorText_ShouldReturnDefault_WhenFlavorTextEntriesHasNoRubyVersionEntry()
        {
            // arrange
            var notRuby = ValidFlavorTextEntry;
            notRuby.Version.Name = "not-ruby";
            var mockedSpecies = new PokemonSpecies
            {
                FlavorTextEntries = new[] { notRuby }
            };

            var pokeApiClientMock = new Mock<IPokeApiClient>();
            pokeApiClientMock.Setup(m => m.GetSpecies(It.IsAny<string>())).ReturnsAsync(mockedSpecies);

            var pokeService = new PokeService(pokeApiClientMock.Object, _pokeApiOptions);

            // act
            var flavorText = await pokeService.GetFlavorText(Charizard);

            // assert
            flavorText.Should().Be(default);
        }

        [Fact]
        public void GetFlavorText_ShouldThrowInvalidOperationException_WhenThereAreMoreThanOneValidFlavorTextEntries()
        {
            // arrange
            var mockedSpecies = new PokemonSpecies
            {
                FlavorTextEntries = new[]
                {
                    ValidFlavorTextEntry,
                    ValidFlavorTextEntry
                }
            };
            var pokeApiClientMock = new Mock<IPokeApiClient>();
            pokeApiClientMock.Setup(m => m.GetSpecies(It.IsAny<string>())).ReturnsAsync(mockedSpecies);

            var pokeService = new PokeService(pokeApiClientMock.Object, _pokeApiOptions);

            Func<Task<string>> getFlavorTextDelegate =
                async () => await pokeService.GetFlavorText(Charizard);

            // act & assert
            getFlavorTextDelegate.Should()
                .Throw<InvalidOperationException>()
                .WithMessage("Sequence contains more than one matching element");
        }

        [Fact]
        public async Task GetFlavorText_ShouldRetrunValidFlavorTextEntry_WhenPokemonSpeciesIsValid()
        {
            // arrange
            var mockedSpecies = new PokemonSpecies
            {
                FlavorTextEntries = new[] { ValidFlavorTextEntry }
            };
            var pokeApiClientMock = new Mock<IPokeApiClient>();
            pokeApiClientMock.Setup(m => m.GetSpecies(It.IsAny<string>())).ReturnsAsync(mockedSpecies);

            var pokeService = new PokeService(pokeApiClientMock.Object, _pokeApiOptions);

            // act
            var flavorText = await pokeService.GetFlavorText(Charizard);

            // assert
            flavorText.Should().Be(ValidFlavorTextEntry.FlavorText);
        }
    }
}
