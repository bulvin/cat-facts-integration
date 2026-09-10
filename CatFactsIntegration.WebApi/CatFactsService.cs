namespace CatFactsIntegration.WebApi;

interface ICatFactsService
{
    Task<CatFact> GetAndStoreAsync(CancellationToken ct = default);
}

public sealed class CatFactsService : ICatFactsService
{
    private readonly ICatFactsClient _client;
    
    public CatFactsService(ICatFactsClient client)
    {
        _client = client;
    }
    
    public async Task<CatFact> GetAndStoreAsync(CancellationToken ct = default)
    {
        var catFact = await _client.GetAsync(ct);

        return catFact;
    }
}