using System;

namespace PokeShake.Api.Services.FunTranslations.Settings
{
    public class FunTranslationsApiOptions
    {
        // FUN_TRANSLATIONS_API_SECRET is pulled from a secret manager and set during CI
        public string ApiSecret { get; } =
            Environment.GetEnvironmentVariable("FUN_TRANSLATIONS_API_SECRET");

        public string BaseUri { get; set; }

        public string ShakespeareanUri { get; set; }
    }
}
