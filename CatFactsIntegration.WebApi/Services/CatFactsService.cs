using CatFactsIntegration.WebApi.Common;
using CatFactsIntegration.WebApi.Domain;
using CatFactsIntegration.WebApi.Storage;

namespace CatFactsIntegration.WebApi.Services;

public interface ICatFactsService
{
    Task<CatFact> GetAndStoreAsync(CancellationToken ct = default);
    Task<PagedResult<CatFact>> GetPagedAsync(int page, int limit, string? phrase = null, CancellationToken ct = default);
}

public sealed class CatFactsService : ICatFactsService
{
    private readonly ICatFactsClient _client;
    private readonly IFileStorage _storage;
    
    public CatFactsService(ICatFactsClient client, IFileStorage storage)
    {
        _client = client;
        _storage = storage;
    }
    
    public async Task<CatFact> GetAndStoreAsync(CancellationToken ct = default)
    {
        var catFact = await _client.GetAsync(ct);
        await _storage.AppendAsync(catFact, ct);

        return catFact;
    }
    
    public Task<PagedResult<CatFact>> GetPagedAsync(int page, int limit, string? phrase, CancellationToken ct = default)
    {
        return _storage.GetPagedAsync(phrase, page, limit, ct);
    }
}