using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using PokeShake.Api.Services.FunTranslations;
using PokeShake.Api.Services.FunTranslations.Interfaces;
using PokeShake.Api.Services.FunTranslations.Models;
using PokeShake.Api.Services.FunTranslations.Settings;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Unit.Services.FunTranslations
{
    [Collection("Services")]
    public class FunTranslationsServiceTests
    {
        private const string Charizard = "Charizard";

        private IOptions<FunTranslationsApiOptions> _pokeApiOptions =
            Options.Create(new FunTranslationsApiOptions());

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task TranslateToShakespearean_ShouldReturnDefault_WhenTextIsNullOrEmpty(string text)
        {
            var loggerMock = new Mock<ILogger<FunTranslationsService>>();
            var apiClientMock = new Mock<IFunTranslationsApiClient>();
            var funTranslationsService = new FunTranslationsService(loggerMock.Object, apiClientMock.Object);

            var translation = await funTranslationsService.TranslateToShakespearean(text);

            translation.Should().Be(default);
            loggerMock.Verify(LogLevel.Warning, "text cannot be empty");
        }

        [Fact]
        public async Task TranslateToShakespearean_ShouldReturnDefault_WhenTranslationResponseIsNull()
        {
            var loggerMock = new Mock<ILogger<FunTranslationsService>>();

            var apiClientMock = new Mock<IFunTranslationsApiClient>();
            var mockedResponse = (FunTranslationsResponse)null;
            apiClientMock.Setup(m => m.TranslateToShakespearean(It.IsAny<string>())).ReturnsAsync(mockedResponse);

            var funTranslationsService = new FunTranslationsService(loggerMock.Object, apiClientMock.Object);

            var translation = await funTranslationsService.TranslateToShakespearean("random-text");

            translation.Should().Be(default);
            loggerMock.Verify(LogLevel.Error, "Fun Translations API failed to translate");
        }

        [Fact]
        public async Task TranslateToShakespearean_ShouldReturnDefault_WhenTranslationResponseSuccessIsNull()
        {
            var loggerMock = new Mock<ILogger<FunTranslationsService>>();
            var apiClientMock = new Mock<IFunTranslationsApiClient>();
            var mockedResponse = new FunTranslationsResponse();
            apiClientMock.Setup(m => m.TranslateToShakespearean(It.IsAny<string>())).ReturnsAsync(mockedResponse);

            var funTranslationsService = new FunTranslationsService(loggerMock.Object, apiClientMock.Object);

            var translation = await funTranslationsService.TranslateToShakespearean("random-text");

            translation.Should().Be(default);
            loggerMock.Verify(LogLevel.Error, "Fun Translations API failed to translate");
        }

        [Fact]
        public async Task TranslateToShakespearean_ShouldReturnDefault_WhenTranslationResponseTotalIsZero()
        {
            var loggerMock = new Mock<ILogger<FunTranslationsService>>();
            var apiClientMock = new Mock<IFunTranslationsApiClient>();
            var mockedResponse = new FunTranslationsResponse()
            {
                Success = new Success { Total = 0 }
            };
            apiClientMock.Setup(m => m.TranslateToShakespearean(It.IsAny<string>())).ReturnsAsync(mockedResponse);

            var funTranslationsService = new FunTranslationsService(loggerMock.Object, apiClientMock.Object);

            var translation = await funTranslationsService.TranslateToShakespearean("random-text");

            translation.Should().Be(default);
            loggerMock.Verify(LogLevel.Error, "Fun Translations API failed to translate");
        }

        [Fact]
        public void TranslateToShakespearean_ShouldThrowNullReferenceException_WhenTranslationResponseContentsIsNull()
        {
            var loggerMock = new Mock<ILogger<FunTranslationsService>>();
            var apiClientMock = new Mock<IFunTranslationsApiClient>();
            var mockedResponse = new FunTranslationsResponse()
            {
                Success = new Success { Total = 1 }
            };
            apiClientMock.Setup(m => m.TranslateToShakespearean(It.IsAny<string>())).ReturnsAsync(mockedResponse);

            var funTranslationsService = new FunTranslationsService(loggerMock.Object, apiClientMock.Object);

            Func<Task<string>> translateDelegate =
                async () => await funTranslationsService.TranslateToShakespearean("random-text");

            translateDelegate.Should().Throw<NullReferenceException>();
        }

        [Fact]
        public async Task TranslateToShakespearean_ShouldReturnExpectedTranslation()
        {
            const string ExpectedTranslation = "Expected Translation";

            var loggerMock = new Mock<ILogger<FunTranslationsService>>();
            var apiClientMock = new Mock<IFunTranslationsApiClient>();
            var mockedResponse = new FunTranslationsResponse()
            {
                Success = new Success { Total = 1 },
                Contents = new Contents { Translated = ExpectedTranslation }
            };
            apiClientMock.Setup(m => m.TranslateToShakespearean(It.IsAny<string>())).ReturnsAsync(mockedResponse);

            var funTranslationsService = new FunTranslationsService(loggerMock.Object, apiClientMock.Object);

            var translation = await funTranslationsService.TranslateToShakespearean("random-text");

            translation.Should().Be(ExpectedTranslation);
        }
    }
}
