namespace RpslsGameService.Infrastructure.Configuration;

public class CachingSettings
{
    public const string SectionName = "Caching";
    
    public TimeSpan DefaultExpiration { get; set; } = TimeSpan.FromMinutes(5);
    public bool EnableDistributedCache { get; set; } = false;
}