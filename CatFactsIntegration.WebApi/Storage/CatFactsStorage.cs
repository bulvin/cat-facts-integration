using System.Text.Json;
using CatFactsIntegration.WebApi.Common;
using CatFactsIntegration.WebApi.Domain;
using CatFactsIntegration.WebApi.Settings;
using Microsoft.Extensions.Options;

namespace CatFactsIntegration.WebApi.Storage;

public interface IFileStorage
{
    Task AppendAsync(CatFact catFact, CancellationToken ct = default);
    Task<PagedResult<CatFact>> GetPagedAsync(string? phrase, int page, int limit, CancellationToken ct = default);
}

public sealed class CatFactsStorage : IFileStorage
{
    private readonly SemaphoreSlim _semaphoreSlim;
    private readonly FileStorageSettings _settings;
    private readonly JsonSerializerOptions _jsonOptions; 
    
    public CatFactsStorage(IOptions<FileStorageSettings> settings)
    {
        _settings = settings.Value;
        _semaphoreSlim = new SemaphoreSlim(1, 1);
        _jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        
        var directoryPath = Path.GetDirectoryName(settings.Value.Path);

        if (!string.IsNullOrWhiteSpace(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
    }

    public async Task AppendAsync(CatFact catFact, CancellationToken ct = default)
    {
        await _semaphoreSlim.WaitAsync(ct);
        
        try
        {
            await using var writer = new StreamWriter(_settings.Path, append: true);
            var json = JsonSerializer.Serialize(catFact, _jsonOptions);
            await writer.WriteLineAsync(json.AsMemory(), ct);
        }
        finally
        {
            _semaphoreSlim.Release();
        }
    }
    
    public async Task<PagedResult<CatFact>> GetPagedAsync(string? phrase, int page, int limit, CancellationToken ct = default)
    {
        await _semaphoreSlim.WaitAsync(ct);

        try
        {
            if (!File.Exists(_settings.Path))
            {
                return new PagedResult<CatFact>([], page, limit, HasNextPage: false);
            }

            var facts = new List<CatFact>(limit + 1);
            var matchingFactsToSkip = (long)(page - 1) * limit;
            long matchingFactsSkipped = 0;

            using var reader = new StreamReader(_settings.Path);
            while (await reader.ReadLineAsync(ct) is { } line)
            {
                var fact = JsonSerializer.Deserialize<CatFact>(
                    line,
                    _jsonOptions);

                if (fact is null)
                {
                    continue;
                }

                var matchesSearch = string.IsNullOrWhiteSpace(phrase) || fact.Fact.Contains(phrase, StringComparison.OrdinalIgnoreCase);
                if (!matchesSearch)
                {
                    continue;
                }
                
                if (matchingFactsSkipped < matchingFactsToSkip)
                {
                    matchingFactsSkipped++;
                    continue;
                }

                facts.Add(fact);

                if (facts.Count > limit)
                {
                    break;
                }
            }
            
            var hasNextPage = facts.Count > limit;
            if (hasNextPage)
            {
                facts.RemoveAt(facts.Count - 1);
            }

            return new PagedResult<CatFact>(facts, page, limit, hasNextPage);
        }
        finally
        {
            _semaphoreSlim.Release();
        }
    }
}