using PokeShake.Api.Services.FunTranslations.Models;
using System.Threading.Tasks;

namespace PokeShake.Api.Services.FunTranslations.Interfaces
{
    /// <summary> A client for <see href="https://funtranslations.com/api/shakespeare">The Shakespearean Fun Translations API</see></summary>
    public interface IFunTranslationsApiClient
    {
        /// <summary> Translates the given text to Shakespearean style </summary>
        /// <param name="text"> The input text to be translated </param>
        Task<FunTranslationsResponse> TranslateToShakespearean(string text);
    }
}
