using System.Threading.Tasks;

namespace PokeShake.Api.Services.Pokemon.Interfaces
{
    public interface IPokeService
    {
        /// <summary> Gets the English flavor text of the Ruby vesion of the requested Pokemon species </summary>
        /// <param name="speciesName"> The requested Pokemon species name </param>
        Task<string> GetFlavorText(string speciesName);
    }
}
