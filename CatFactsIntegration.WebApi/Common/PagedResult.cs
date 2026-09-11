namespace CatFactsIntegration.WebApi.Common;

public sealed record PagedResult<T>(IReadOnlyList<T> Items, int Page, int Limit, bool HasNextPage);