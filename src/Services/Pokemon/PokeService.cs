using Microsoft.Extensions.Options;
using PokeShake.Api.Services.Pokemon.Interfaces;
using PokeShake.Api.Services.Pokemon.Settings;
using System.Linq;
using System.Threading.Tasks;

namespace PokeShake.Api.Services.Pokemon
{
    public class PokeService : IPokeService
    {
        private readonly IPokeApiClient _pokeApiClient;
        private readonly PokeApiOptions _pokeApiOptions;

        public PokeService(
            IPokeApiClient pokeApiClient,
            IOptions<PokeApiOptions> pokeApiOptions)
        {
            _pokeApiClient = pokeApiClient;
            _pokeApiOptions = pokeApiOptions.Value;
        }

        public async Task<string> GetFlavorText(string speciesName)
        {
            var species = await _pokeApiClient.GetSpecies(speciesName);

            // when no species is found or rate limit is reached
            if (species == default)
                return default;

            var flavorTextEntry = species.FlavorTextEntries.SingleOrDefault(fte =>
                fte.Version.Name == _pokeApiOptions.GameVersion &&
                fte.Language.Name == _pokeApiOptions.Language);

            // returns default if no match found
            return flavorTextEntry?.FlavorText;
        }
    }
}
