using CatFactsIntegration.WebApi.Domain;
using CatFactsIntegration.WebApi.Settings;
using CatFactsIntegration.WebApi.Storage;
using Microsoft.Extensions.Options;

namespace CatFactsIntegration.Tests;

public class CatFactsStorageTests : IDisposable
{
    [Fact]
    public async Task AppendAsync_ShouldStoreCatFact()
    {
   
        var expectedFact = new CatFact("Cats can jump.", 14);
        
        await _storage.AppendAsync(expectedFact);

        var result = await _storage.GetPagedAsync(page: 1, limit: 10, phrase: null);
        
        Assert.Single(result.Items);
        Assert.Equal(expectedFact, result.Items[0]);
    }

    [Fact]
    public async Task GetPagedAsync_ShouldSearchAndPaginateCatFacts()
    {
     
        await AppendFactsAsync(
            new CatFact("Cat one.", 13),
            new CatFact("Dog one.", 9),
            new CatFact("Parrot one.", 13),
            new CatFact("Cat two.", 15));

        var result = await _storage.GetPagedAsync(
            page: 1,
            limit: 1,
            phrase: "parrot");

     
        Assert.Single(result.Items);
        Assert.Equal("Parrot one.", result.Items[0].Fact);
    }
    
    #region arrange
    
    private readonly string _filePath;
    private readonly CatFactsStorage _storage;

    public CatFactsStorageTests()
    {
        _filePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.txt");

        var settings = Options.Create(new FileStorageSettings 
        {
            Path = _filePath
            
        });

        _storage = new CatFactsStorage(settings);
    }
    
    private async Task AppendFactsAsync(params CatFact[] facts)
    {
        foreach (var fact in facts)
        {
            await _storage.AppendAsync(fact);
        }
    }

    #endregion
    
    public void Dispose()
    {
        if (File.Exists(_filePath))
        {
            File.Delete(_filePath);
        }
    }
}