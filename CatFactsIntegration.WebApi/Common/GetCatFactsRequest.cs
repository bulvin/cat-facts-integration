namespace CatFactsIntegration.WebApi.Common;

public record GetCatFactsRequest(
    int Page = 1,
    int Limit = 50,
    string? Phrase = null);