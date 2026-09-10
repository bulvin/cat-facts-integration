namespace CatFactsIntegration.WebApi.Settings;

public sealed class CatFactsSettings
{
    public const string SectionName = "CatFactsApi";

    public required string BaseUrl { get; init; }
}