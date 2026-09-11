using CatFactsIntegration.WebApi.Common;
using CatFactsIntegration.WebApi.Services;

namespace CatFactsIntegration.WebApi.Endpoints;

public static class CatFactsEndpoints
{
    public static IEndpointRouteBuilder MapCatFactsEndpoints(
        this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/facts", GetCatFacts);
        endpoints.MapGet("/fact", GetCatFact);
        endpoints.MapGet("/", () => TypedResults.Redirect("/fact"));
        return endpoints;
    }
    
    private static async Task<IResult> GetCatFact(
        ICatFactsService service,
        CancellationToken ct)
    {
        var response = await service.GetAndStoreAsync(ct);

        return TypedResults.Ok(response);
    }

    private static async Task<IResult> GetCatFacts(
        [AsParameters] GetCatFactsRequest request,
        ICatFactsService service,
        CancellationToken ct)
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
    
        var response = await service.GetPagedAsync(page, limit, phrase?.Trim(), ct);
        return TypedResults.Ok(response);
    }

    
}