using Microsoft.Extensions.Logging;
using PokeShake.Api.Services.FunTranslations.Interfaces;
using System.Threading.Tasks;

namespace PokeShake.Api.Services.FunTranslations
{
    public class FunTranslationsService : IFunTranslationsService
    {
        private readonly ILogger<FunTranslationsService> _logger;
        private readonly IFunTranslationsApiClient _funTranslationsApiClient;

        public FunTranslationsService(
            ILogger<FunTranslationsService> logger,
            IFunTranslationsApiClient funTranslationsApiClient)
        {
            _logger = logger;
            _funTranslationsApiClient = funTranslationsApiClient;
        }

        public async Task<string> TranslateToShakespearean(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                _logger.LogWarning("text cannot be empty");
                return default;
            }

            var response = await _funTranslationsApiClient.TranslateToShakespearean(text);

            if ((response?.Success?.Total ?? 0) != 1)
            {
                _logger.LogError("Fun Translations API failed to translate");
                return default;
            }

            return response.Contents.Translated;
        }
    }
}
