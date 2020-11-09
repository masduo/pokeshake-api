using System.Text.Json.Serialization;

namespace PokeShake.Api.Services.Pokemon.Models
{
    /// <summary>
    /// A flavor text entry for a Pokémon species as descibed
    /// <see href="https://pokeapi.co/docs/v2#pokemon-species">here</see>
    /// </summary>
    public class FlavorTextEntry
    {
        /// <summary> Gets or sets the localized flavor text for an API resource </summary>
        [JsonPropertyName("flavor_text")]
        public string FlavorText { get; set; }

        /// <summary> Gets or sets the language this name is in </summary>
        public NamedApiResource Language { get; set; }

        /// <summary> Gets or sets the game version this flavor text is extracted from </summary>
        public NamedApiResource Version { get; set; }
    }
}
