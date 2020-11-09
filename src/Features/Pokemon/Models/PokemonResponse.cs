namespace PokeShake.Api.Features.Pokemon
{
    public class PokemonResponse
    {
        /// <summary> Gets or sets Pokemon species name </summary>
        /// <example> charizard </example>
        public string Name { get; set; }

        /// <summary> Gets or sets Pokemon species flavor text translated to Shakespearean style </summary>
        /// <example> Charizard flies 'round the sky in search of powerful opponents. 't breathes fire of such most wondrous heat yond 't melts aught. However,  't nev'r turns its fiery breath on any opponent weaker than itself. </example>
        public string Description { get; set; }
    }
}