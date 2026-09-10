using System.Text.Json;
using CatFactsIntegration.WebApi.Settings;
using Microsoft.Extensions.Options;

namespace CatFactsIntegration.WebApi.Storage;

public interface IFileStorage
{
    Task AppendAsync(CatFact catFact, CancellationToken ct = default);
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
        catch (UnauthorizedAccessException ex)
        {
            Console.WriteLine("Access Denied: " + ex.Message);
        }
        catch (IOException ex)
        {
            Console.WriteLine("Disk Error: " + ex.Message);
        }
        finally
        {
            _semaphoreSlim.Release();
        }
    }
}