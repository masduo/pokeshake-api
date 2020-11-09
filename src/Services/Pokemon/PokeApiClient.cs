using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PokeShake.Api.Services.Pokemon.Interfaces;
using PokeShake.Api.Services.Pokemon.Models;
using PokeShake.Api.Services.Pokemon.Settings;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace PokeShake.Api.Services.Pokemon
{
    public class PokeApiClient : IPokeApiClient
    {
        private static JsonSerializerOptions _serializationOptions =
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

        private readonly ILogger<PokeApiClient> _logger;
        private readonly PokeApiOptions _apiOptions;
        private readonly HttpClient _httpClient;

        public PokeApiClient(
            ILogger<PokeApiClient> logger,
            IOptions<PokeApiOptions> apiOptions,
            HttpClient httpClient)
        {
            _logger = logger;
            _apiOptions = apiOptions.Value;
            _httpClient = httpClient;

            _httpClient.BaseAddress = new Uri(_apiOptions.BaseUri);
        }

        public async Task<PokemonSpecies> GetSpecies(string speciesName)
        {
            var uri = $"{_apiOptions.SpeciesUri}/{speciesName}";

            var response = await _httpClient.GetAsync(uri);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogInformation("No Pokemon species found for '{name}'", speciesName);
                return default;
            }

            response.EnsureSuccessStatusCode();

            var species = await JsonSerializer.DeserializeAsync<PokemonSpecies>(
                await response.Content.ReadAsStreamAsync(), _serializationOptions);

            return species;
        }
    }
}
