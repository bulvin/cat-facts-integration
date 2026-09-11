using System.Net;
using CatFactsIntegration.WebApi;
using CatFactsIntegration.WebApi.Common;
using CatFactsIntegration.WebApi.Services;
using CatFactsIntegration.WebApi.Settings;
using CatFactsIntegration.WebApi.Storage;
using Microsoft.AspNetCore.Mvc;
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

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
    
    app.MapGet("/test-exception/{type}", (string type) =>
    {
        throw type switch
        {
            "unauthorized" => new UnauthorizedAccessException(
                "Test access denied."),

            "io" => new IOException(
                "Test file write failure."),

            "external-404" => new HttpRequestException(
                "Cat fact not found.",
                inner: null,
                statusCode: HttpStatusCode.NotFound),

            "external-500" => new HttpRequestException(
                "External server error.",
                inner: null,
                statusCode: HttpStatusCode.InternalServerError),

            _ => new InvalidOperationException(
                "Test unexpected exception.")
        };
    });
}

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.MapGet("/", () => "Hello World!");

app.MapGet("/fact", async (ICatFactsService service, CancellationToken ct) =>
{
    var response = await service.GetAndStoreAsync(ct);
    return Results.Ok(response);
});

app.MapGet("/facts", async (
    ICatFactsService storage,
    CancellationToken ct,
    [AsParameters] GetCatFactsRequest request
    ) =>
{
    var (page, limit, phrase) = request;
    if (request.Page < 1)
    {
        page = 1;
    }

    if (limit is < 1 or > 100)
    {
        limit = 100;
    }
    
    var response = await storage.GetPagedAsync(page, limit, phrase?.Trim(), ct);
    return Results.Ok(response);
});

app.Run();