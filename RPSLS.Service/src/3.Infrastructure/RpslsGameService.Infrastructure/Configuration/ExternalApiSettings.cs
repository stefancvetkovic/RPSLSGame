namespace RpslsGameService.Infrastructure.Configuration;

public class ExternalApiSettings
{
    public const string SectionName = "ExternalApis";
    
    public RandomNumberServiceSettings RandomNumberService { get; set; } = new();
}

public class RandomNumberServiceSettings
{
    public string BaseUrl { get; set; } = "https://codechallenge.boohma.com";
    public int TimeoutSeconds { get; set; } = 30;
    public int RetryCount { get; set; } = 3;
    public bool EnableFallback { get; set; } = true;
}