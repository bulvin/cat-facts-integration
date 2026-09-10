using CatFactsIntegration.WebApi.Storage;

namespace CatFactsIntegration.WebApi;

public interface ICatFactsService
{
    Task<CatFact> GetAndStoreAsync(CancellationToken ct = default);
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
}