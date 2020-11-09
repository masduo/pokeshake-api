using PokeShake.Api.Services.Pokemon.Models;
using System.Net.Http;
using System.Threading.Tasks;

namespace PokeShake.Api.Services.Pokemon.Interfaces
{
    /// <summary> A client for <see href="https://pokeapi.co/docs/v2">Poke API</see></summary>
    public interface IPokeApiClient
    {
        /// <summary> Calls Poke API and gets the flavor text of the given species </summary>
        /// <param name="speciesName"> The name of the species </param>
        Task<PokemonSpecies> GetSpecies(string speciesName);
    }
}
