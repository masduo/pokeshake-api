using PokeShake.Api.Services.Pokemon.Interfaces;
using System.Threading.Tasks;

namespace Tests.Integration.Fakes
{
    public class FakePokeService : IPokeService
    {
        private string _fakeFlavor;

        public FakePokeService(string fakeFlavor = "Fake Flavor")
        {
            _fakeFlavor = fakeFlavor;
        }

        public async Task<string> GetFlavorText(string name)
        {
            await Task.CompletedTask;

            return _fakeFlavor;
        }
    }
}
