using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace CatFactsIntegration.WebApi;

public sealed class GlobalExceptionHandler(IProblemDetailsService problemDetailsService, ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Unhandled exception occurred. TraceId: {TraceId}",
            httpContext.TraceIdentifier);
        
        var (statusCode, title) = MapException(exception);

        httpContext.Response.StatusCode = statusCode;

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Instance = httpContext.Request.Path,
            Detail = GetSafeErrorMessage(exception, httpContext),
            Extensions =
            {
                ["traceId"] = httpContext.TraceIdentifier,
                ["timestamp"] = DateTime.UtcNow
            }
        };

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problemDetails
        });
    }

    private static (int StatusCode, string Title) MapException(Exception exception) => exception switch
    {
        BadHttpRequestException => (StatusCodes.Status400BadRequest, "Invalid Request"),
        UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Access denied"),
        HttpRequestException { StatusCode: HttpStatusCode.NotFound } => (StatusCodes.Status404NotFound,
            "External resource not found"),
        HttpRequestException => (StatusCodes.Status502BadGateway, "External cat facts service error"),
        IOException => (StatusCodes.Status500InternalServerError, "Disk error"),
        _ => (StatusCodes.Status500InternalServerError, "Unexpected error")
    };
    
    private static string? GetSafeErrorMessage(Exception exception, HttpContext context)
    {
        var env = context.RequestServices.GetRequiredService<IHostEnvironment>();
        return env.IsDevelopment() ? exception.Message : null;
    }
}