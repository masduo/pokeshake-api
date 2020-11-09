using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PokeShake.Api.Services.FunTranslations.Interfaces;
using PokeShake.Api.Services.FunTranslations.Models;
using PokeShake.Api.Services.FunTranslations.Settings;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace PokeShake.Api.Services.FunTranslations
{
    public class FunTranslationsApiClient : IFunTranslationsApiClient
    {
        // todo move to options
        private const string ApiSecretHeaderName = "X-Funtranslations-Api-Secret";
        private const string RateLimitHeaderName = "X-RateLimit-Limit";
        private const string RateLimitRemainingHeaderName = "X-RateLimit-Remaining";

        private static JsonSerializerOptions _serializationOptions =
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

        private readonly ILogger<FunTranslationsApiClient> _logger;
        private readonly FunTranslationsApiOptions _apiOptions;
        private readonly HttpClient _httpClient;

        public FunTranslationsApiClient(
            ILogger<FunTranslationsApiClient> logger,
            IOptions<FunTranslationsApiOptions> apiOptions,
            HttpClient httpClient)
        {
            _logger = logger;
            _apiOptions = apiOptions.Value;
            _httpClient = httpClient;

            _httpClient.BaseAddress = new Uri(_apiOptions.BaseUri);
            _httpClient.DefaultRequestHeaders.Add(ApiSecretHeaderName, _apiOptions.ApiSecret);
        }

        public async Task<FunTranslationsResponse> TranslateToShakespearean(string text)
        {
            // input text may have non-ascii characters
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>(nameof(text), text)
            });

            var response = await _httpClient.PostAsync(_apiOptions.ShakespeareanUri, content);

            if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
            {
                _logger.LogError("Fun Translations API rate limit is reached");
                return default;
            }

            if (response.Headers.TryGetValues(RateLimitHeaderName, out var rateLimit))
            {
                _logger.LogInformation("Fun Translations API rate limit '{@rateLimit}'", rateLimit);
            }

            if (response.Headers.TryGetValues(RateLimitRemainingHeaderName, out var rateLimitRemaining))
            {
                _logger.LogInformation("Fun Translations API remaining allowance {@remaining}", rateLimitRemaining);
            }

            response.EnsureSuccessStatusCode();

            var funTranslationsResponse = await JsonSerializer.DeserializeAsync<FunTranslationsResponse>(
                await response.Content.ReadAsStreamAsync(), _serializationOptions);

            return funTranslationsResponse;
        }
    }
}
