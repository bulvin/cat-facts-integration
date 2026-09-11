namespace CatFactsIntegration.WebApi.Common;

public sealed record GetCatFactsRequest(
    int Page = 1,
    int Limit = 50,
    string? Phrase = null);