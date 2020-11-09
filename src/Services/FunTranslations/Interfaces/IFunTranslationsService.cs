using System.Threading.Tasks;

namespace PokeShake.Api.Services.FunTranslations.Interfaces
{
    public interface IFunTranslationsService
    {
        Task<string> TranslateToShakespearean(string text);
    }
}
