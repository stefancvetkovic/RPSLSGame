using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;
using RpslsGameService.Application.Interfaces;
using RpslsGameService.Infrastructure.Caching;
using RpslsGameService.Infrastructure.Configuration;
using RpslsGameService.Infrastructure.ExternalServices;
using RpslsGameService.Infrastructure.Persistence;
using Serilog;

namespace RpslsGameService.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddConfiguration(configuration);
        services.AddExternalServices(configuration);
        services.AddPersistence();
        services.AddCaching();
        services.AddLogging();

        return services;
    }

    private static IServiceCollection AddConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ExternalApiSettings>(configuration.GetSection(ExternalApiSettings.SectionName));
        services.Configure<CachingSettings>(configuration.GetSection(CachingSettings.SectionName));

        return services;
    }

    private static IServiceCollection AddExternalServices(this IServiceCollection services, IConfiguration configuration)
    {
        var externalApiSettings = configuration.GetSection(ExternalApiSettings.SectionName).Get<ExternalApiSettings>();
        var randomNumberSettings = externalApiSettings?.RandomNumberService ?? new RandomNumberServiceSettings();

        services.AddHttpClient<IRandomNumberService, HttpRandomNumberService>(client =>
        {
            client.BaseAddress = new Uri(randomNumberSettings.BaseUrl);
            client.Timeout = TimeSpan.FromSeconds(randomNumberSettings.TimeoutSeconds);
        })
        .AddPolicyHandler(GetRetryPolicy(randomNumberSettings.RetryCount))
        .AddPolicyHandler(GetCircuitBreakerPolicy());

        return services;
    }

    private static IServiceCollection AddPersistence(this IServiceCollection services)
    {
        services.AddSingleton<IGameSessionRepository, InMemoryGameSessionRepository>();

        return services;
    }

    private static IServiceCollection AddCaching(this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddSingleton<ICacheService, InMemoryCacheService>();

        return services;
    }

    private static IServiceCollection AddLogging(this IServiceCollection services)
    {
        var logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File("logs/rpsls-game-.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        services.AddSingleton<Serilog.ILogger>(logger);

        return services;
    }

    private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(int retryCount)
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(
                retryCount,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (outcome, timespan, retryCount, context) =>
                {
                    var logger = context.GetLogger();
                    logger?.LogWarning("Retry {RetryCount} after {Delay}ms delay due to: {Exception}",
                        retryCount, timespan.TotalMilliseconds, outcome.Exception?.Message);
                });
    }

    private static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(
                exceptionsAllowedBeforeBreaking: 3,
                durationOfBreak: TimeSpan.FromSeconds(30),
                onBreak: (exception, duration) =>
                {
                    // Log circuit breaker opening
                },
                onReset: () =>
                {
                    // Log circuit breaker closing
                });
    }

    private static ILogger? GetLogger(this Context context)
    {
        if (context.TryGetValue("logger", out var logger) && logger is ILogger typedLogger)
        {
            return typedLogger;
        }
        return null;
    }
}