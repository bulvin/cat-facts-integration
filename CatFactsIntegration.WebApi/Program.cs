using CatFactsIntegration.WebApi;
using CatFactsIntegration.WebApi.Settings;
using CatFactsIntegration.WebApi.Storage;
using Microsoft.Extensions.Options;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services
    .AddOptions<CatFactsSettings>()
    .Bind(builder.Configuration.GetSection(CatFactsSettings.SectionName))
    .Validate(options =>
            Uri.TryCreate(options.BaseUrl, UriKind.Absolute, out _),
        "CatFactsApi:BaseUrl must be a valid absolute URL.")
    .ValidateOnStart();

builder.Services
    .AddOptions<FileStorageSettings>()
    .Bind(builder.Configuration.GetSection(FileStorageSettings.SectionName))
    .Validate(
        options => !string.IsNullOrWhiteSpace(options.Path),
        "FileStorage:Path is required")
    .ValidateOnStart();

builder.Services.AddHttpClient<ICatFactsClient, CatFactsClient>((serviceProvider, client) =>
{
    var options = serviceProvider
        .GetRequiredService<IOptions<CatFactsSettings>>()
        .Value;
    
    client.BaseAddress = new Uri(options.BaseUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder.Services.AddScoped<ICatFactsService, CatFactsService>();

builder.Services.AddSingleton<IFileStorage, CatFactsStorage>();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.MapGet("/", () => "Hello World!");

app.MapGet("/fact", async (ICatFactsService service, CancellationToken ct) =>
{
    var response = await service.GetAndStoreAsync(ct);
    return Results.Ok(response);
});

app.Run();