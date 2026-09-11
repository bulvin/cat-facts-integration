using CatFactsIntegration.WebApi.Common;
using CatFactsIntegration.WebApi.Domain;
using CatFactsIntegration.WebApi.Services;
using CatFactsIntegration.WebApi.Storage;
using NSubstitute;

namespace CatFactsIntegration.Tests;

public class CatFactsServiceTests
{
    [Fact]
    public async Task GetAndStoreAsync_ShouldGetAndStoreCatFact()
    {
        var expectedFact = new CatFact("Cats like parrots", 14);
        
        _client.GetAsync(Arg.Any<CancellationToken>())
            .Returns(expectedFact);

        var result = await _service.GetAndStoreAsync();
        
        Assert.Equal(expectedFact, result);
        
        await _client.Received(1)
            .GetAsync(Arg.Any<CancellationToken>());
        
        await _storage.Received(1)
            .AppendAsync(expectedFact, Arg.Any<CancellationToken>());
    }
    
    [Fact]
    public async Task GetPagedAsync_ShouldReturnPagedFromStorage()
    {
        const int page = 2;
        const int limit = 10;
        const string phrase = "cat";

        var expectedResult = new PagedResult<CatFact>([new CatFact("Cats like parrots", 14)], page, limit, false);

        _storage.GetPagedAsync(phrase, page, limit, Arg.Any<CancellationToken>())
            .Returns(expectedResult);
        
        var result = await _service.GetPagedAsync(page, limit, phrase);
        
        Assert.Equal(expectedResult, result);

        await _storage.Received(1)
            .GetPagedAsync(phrase, page, limit, Arg.Any<CancellationToken>());
    }
    
    
    #region arrange

    private readonly ICatFactsClient _client;
    private readonly IFileStorage _storage;
    private readonly CatFactsService _service;

    public CatFactsServiceTests()
    {
        _client = Substitute.For<ICatFactsClient>();
        _storage = Substitute.For<IFileStorage>();
        
        _service = new CatFactsService(_client, _storage);
    }
    
    #endregion
    
}