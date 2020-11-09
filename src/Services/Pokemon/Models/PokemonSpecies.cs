using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PokeShake.Api.Services.Pokemon.Models
{
    /// <summary>
    /// A Pokémon Species forms the basis for at least one Pokémon.
    /// Attributes of a Pokémon species are shared across all varieties of
    /// Pokémon within the species. A good example is Wormadam; Wormadam is
    /// the species which can be found in three different varieties,
    /// Wormadam-Trash, Wormadam-Sandy and Wormadam-Plant.
    /// See <a href="https://pokeapi.co/docs/v2#pokemon-species">here</a>
    /// for more details.
    /// </summary>
    public class PokemonSpecies
    {
        /// <summary> Gets or sets a list of flavor text entries for this Pokémon species </summary>
        [JsonPropertyName("flavor_text_entries")]
        public IEnumerable<FlavorTextEntry> FlavorTextEntries { get; set; }
    }
}
