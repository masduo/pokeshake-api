namespace PokeShake.Api.Services.FunTranslations.Models
{
    /// <summary> Fun Translations API's response content schema.
    /// Read <see href="https://funtranslations.com/api/shakespeare">here</see> for more information
    /// </summary>
    public class FunTranslationsResponse
    {
        /// <summary> Gets or sets the success state of the response </summary>
        public Success Success { get; set; }

        /// <summary> Gets or sets the content of the response </summary>
        public Contents Contents { get; set; }
    }
}
