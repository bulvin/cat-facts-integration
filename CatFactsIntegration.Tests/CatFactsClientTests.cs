using System.Net;
using System.Text;
using System.Text.Json;
using CatFactsIntegration.WebApi.Domain;
using CatFactsIntegration.WebApi.Services;

namespace CatFactsIntegration.Tests;

public class CatFactsClientTests
{
    [Fact]
    public async Task GetAsync_ShouldDeserializeCatFact()
    {
        var expectedFact = new CatFact("Cats like parrots.", 14);
        
        var client = CreateClient(HttpStatusCode.OK, expectedFact);
        
        var result = await client.GetAsync();
        
        Assert.NotNull(result);
        Assert.Equal("Cats like parrots.", result.Fact);
        Assert.Equal(14, result.Length);
    }

    [Theory]
    [InlineData(HttpStatusCode.NotFound)]
    [InlineData(HttpStatusCode.InternalServerError)]
    public async Task GetAsync_ShouldThrowHttpRequestException_WhenApiReturnsError(HttpStatusCode statusCode)
    {
        var client = CreateClient(statusCode);
        
        var exception = await Assert.ThrowsAsync<HttpRequestException>(() => client.GetAsync());
        
        Assert.Equal(statusCode, exception.StatusCode);
    }
    
    #region arrange
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private class StubHttpMessageHandler(HttpResponseMessage response) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(response);
        }
    }
    
    private static CatFactsClient CreateClient(HttpStatusCode statusCode, CatFact? response = null)
    {
        var httpResponse = new HttpResponseMessage(statusCode);
    
        if (response is not null)
        {
            var json = JsonSerializer.Serialize(response, JsonOptions);
            httpResponse.Content = new StringContent(json, Encoding.UTF8, "application/json");
        }
    
        var handler = new StubHttpMessageHandler(httpResponse);
    
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://cat-facts.test")
        };
    
        return new CatFactsClient(httpClient);
    }
    
    #endregion

}