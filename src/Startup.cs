using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using PokeShake.Api.Services.FunTranslations;
using PokeShake.Api.Services.FunTranslations.Interfaces;
using PokeShake.Api.Services.FunTranslations.Settings;
using PokeShake.Api.Services.Pokemon;
using PokeShake.Api.Services.Pokemon.Interfaces;
using PokeShake.Api.Services.Pokemon.Settings;
using Serilog;
using System;
using System.IO;

namespace PokeShake.Api
{
    public class Startup
    {
        public const string DefaultApiVersion = "1.0";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddHealthChecks();

            services.AddMemoryCache();

            services
                .Configure<PokeApiOptions>(Configuration.GetSection(nameof(PokeApiOptions)))
                .Configure<FunTranslationsApiOptions>(Configuration.GetSection(nameof(FunTranslationsApiOptions)));

            services.AddHttpClient<IPokeApiClient, PokeApiClient>();
            services.AddHttpClient<IFunTranslationsApiClient, FunTranslationsApiClient>();

            services
                .AddTransient<IPokeService, PokeService>()
                .AddTransient<IFunTranslationsService, FunTranslationsService>();

            services
                .AddVersionedApiExplorer(options =>
                {
                    options.GroupNameFormat = "VV";
                    options.SubstituteApiVersionInUrl = true;
                    options.AssumeDefaultVersionWhenUnspecified = true;
                    options.DefaultApiVersion = ApiVersion.Parse(DefaultApiVersion);
                })
                .AddApiVersioning(options => options.ReportApiVersions = true);

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc(DefaultApiVersion, new OpenApiInfo
                {
                    Title = "PokeShake API",
                    Version = DefaultApiVersion,
                    Description = "A fresh perspective at the world of Pokemon!",
                    Contact = new OpenApiContact { Url = new Uri("https://github.com/masduo/pokeshake-api") }
                });

                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "PokeShake.Api.xml"));
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider vdp)
        {
            app.UseSerilogRequestLogging();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/healthcheck");
            });

            app.UseSwagger();

            app.UseSwaggerUI(options =>
            {
                foreach (var description in vdp.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint(
                       $"/swagger/{description.GroupName}/swagger.json",
                       $"PokeShake API {description.GroupName}");
                }
            });
        }
    }
}
