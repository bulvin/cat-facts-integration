using System.Net;
using CatFactsIntegration.WebApi.Endpoints;
using Scalar.AspNetCore;

namespace CatFactsIntegration.WebApi;

public static class ConfigureApp
{
    public static WebApplication Configure(
        this WebApplication app)
    {
        app.UseExceptionHandler();
        app.UseHttpsRedirection();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
            app.MapTestExceptionEndpoint();
        }

        app.MapCatFactsEndpoints();

        return app;
    }

    private static void MapTestExceptionEndpoint(
        this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/test-exception/{type}", (string type) =>
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
}