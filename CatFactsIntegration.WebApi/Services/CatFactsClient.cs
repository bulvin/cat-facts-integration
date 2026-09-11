namespace CatFactsIntegration.WebApi.Services;

public interface ICatFactsClient
{
    Task<CatFact> GetAsync(CancellationToken ct = default);
}

public sealed class CatFactsClient : ICatFactsClient
{
    private readonly HttpClient _http;

    public CatFactsClient(HttpClient http)
    {
        _http = http;
    }
    
    public async Task<CatFact> GetAsync(CancellationToken ct = default)
    {
        var response = await _http.GetAsync("fact", ct);
        
        response.EnsureSuccessStatusCode();
        
        var catFact = await response.Content.ReadFromJsonAsync<CatFact>(ct)
                      ?? throw new InvalidOperationException($"Cannot deserialize catFact from API response: {response.Content}");
       
        return catFact;
    }
}