namespace PokeShake.Api.Services.FunTranslations.Models
{
    public class Contents
    {
        /// <summary>Gets or sets the input text that was sent in the request to be translated </summary>
        public string Text { get; set; }

        /// <summary> Gets or sets the translated text </summary>
        public string Translated { get; set; }

        /// <summary> Gets or sets the translation language or style, e.g. `Shakespeare`</summary>
        public string Translation { get; set; }
    }
}
