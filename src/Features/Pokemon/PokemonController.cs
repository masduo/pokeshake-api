using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using PokeShake.Api.Services.FunTranslations.Interfaces;
using PokeShake.Api.Services.Pokemon.Interfaces;
using System;
using System.Net.Mime;
using System.Threading.Tasks;

namespace PokeShake.Api.Features.Pokemon
{
    [ApiController]
    public class PokemonController : ControllerBase
    {
        private TimeSpan _expireIn12Hours = new TimeSpan(TimeSpan.TicksPerDay / 2);
        private readonly ILogger<PokemonController> _logger;
        private readonly IMemoryCache _memoeryCache;
        private readonly IPokeService _pokeService;
        private readonly IFunTranslationsService _funTranslationsService;

        public PokemonController(
            ILogger<PokemonController> logger,
            IMemoryCache memoeryCache,
            IPokeService pokeService,
            IFunTranslationsService funTranslationsService)
        {
            _logger = logger;
            _memoeryCache = memoeryCache;
            _pokeService = pokeService;
            _funTranslationsService = funTranslationsService;
        }

        /// <summary> Gets a Pokemon species name, and translates its flavor text to Shakespearean style </summary>
        /// <param name="speciesName" example="charizard">
        /// Pokemon species name. It is case sensitive.
        /// For a comprehensive list of available sepecies refer to <a href="https://pokeapi.co/api/v2/pokemon-species">here</a>
        /// </param>
        /// <response code="400"> `BadRequest` when speciesName is empty or whitespace </response>
        /// <response code="404"> `NotFound` when no Pokemon species is found </response>
        [HttpGet]
        [ApiVersion(Startup.DefaultApiVersion)]
        [Route("/v{version:apiVersion}/pokemon/{speciesName}")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(PokemonResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> Translate([FromRoute] string speciesName)
        {
            if (string.IsNullOrWhiteSpace(speciesName))
                return BadRequest("Name must be provided");

            if (_memoeryCache.TryGetValue(speciesName, out var cachedPayload))
            {
                _logger.LogInformation("Using the cache entry of '{speciedName}'", speciesName);
                return Ok(cachedPayload);
            }

            var flavorText = await _pokeService.GetFlavorText(speciesName);

            if (flavorText == default)
                return NotFound($"No Pokemon species found for '{speciesName}'");

            var translation = await _funTranslationsService.TranslateToShakespearean(flavorText);

            if (translation == default)
                return NotFound($"Translation failed for '{speciesName}', or rate limit is reached");

            // expire cache in 12 hours to align with Fun Translations API's cache max-age
            _logger.LogInformation("Setting a cache entry for '{speciedName}' response", speciesName);
            return Ok(_memoeryCache.Set(speciesName,
                new PokemonResponse
                {
                    Name = speciesName,
                    Description = translation
                },
                _expireIn12Hours));
        }
    }
}
