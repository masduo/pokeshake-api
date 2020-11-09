using PokeShake.Api.Services.FunTranslations.Interfaces;
using System.Threading.Tasks;

namespace Tests.Integration.Fakes
{
    public class FakeFunTranslationsService : IFunTranslationsService
    {
        private string _fakeTranslation;

        public FakeFunTranslationsService(string fakeTranslation = "Fake Translation")
        {
            _fakeTranslation = fakeTranslation;
        }

        public async Task<string> TranslateToShakespearean(string text)
        {
            await Task.CompletedTask;

            return _fakeTranslation;
        }
    }
}
