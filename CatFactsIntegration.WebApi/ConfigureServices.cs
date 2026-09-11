using CatFactsIntegration.WebApi.Services;
using CatFactsIntegration.WebApi.Settings;
using CatFactsIntegration.WebApi.Storage;
using Microsoft.Extensions.Options;

namespace CatFactsIntegration.WebApi;

public static class ConfigureServices
{
    public static IServiceCollection AddServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOpenApi();

        services
            .AddOptions<CatFactsSettings>()
            .Bind(configuration.GetSection(CatFactsSettings.SectionName))
            .Validate(
                options => Uri.TryCreate(
                    options.BaseUrl,
                    UriKind.Absolute,
                    out _),
                "CatFacts:BaseUrl must be a valid absolute URL.")
            .ValidateOnStart();

        services
            .AddOptions<FileStorageSettings>()
            .Bind(configuration.GetSection(FileStorageSettings.SectionName))
            .Validate(
                options => !string.IsNullOrWhiteSpace(options.Path),
                "FileStorage:Path is required.")
            .ValidateOnStart();

        services.AddHttpClient<ICatFactsClient, CatFactsClient>(
            (serviceProvider, client) =>
            {
                var settings = serviceProvider
                    .GetRequiredService<IOptions<CatFactsSettings>>()
                    .Value;

                client.BaseAddress = new Uri(settings.BaseUrl);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });

        services.AddScoped<ICatFactsService, CatFactsService>();
        services.AddSingleton<IFileStorage, CatFactsStorage>();

        services.AddProblemDetails();
        services.AddExceptionHandler<GlobalExceptionHandler>();

        return services;
    }
}