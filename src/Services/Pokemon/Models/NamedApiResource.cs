namespace PokeShake.Api.Services.Pokemon.Models
{
    /// <summary>
    /// A Named API Resource as described
    /// <see href="https://pokeapi.co/docs/v2#namedapiresource"> here </see>
    /// </summary>
    public class NamedApiResource
    {
        /// <summary> Gets or sets the name of the referenced resource </summary>
        public string Name { get; set; }

        /// <summary> Gets or sets the URL of the referenced resource </summary>
        public string Url { get; set; }
    }
}
