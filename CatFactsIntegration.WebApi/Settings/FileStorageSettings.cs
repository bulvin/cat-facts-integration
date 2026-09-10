namespace CatFactsIntegration.WebApi.Settings;

public sealed class FileStorageSettings
{
    public const string SectionName = "FileStorage";
    
    public required string Path { get; init; }
}