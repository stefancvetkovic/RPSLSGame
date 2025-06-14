using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using RpslsGameService.Application.Interfaces;
using RpslsGameService.Infrastructure.Configuration;
using System.Text.Json;

namespace RpslsGameService.Infrastructure.ExternalServices;

public class HttpRandomNumberService : IRandomNumberService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<HttpRandomNumberService> _logger;
    private readonly RandomNumberServiceSettings _settings;
    private readonly Random _fallbackRandom = new();

    public HttpRandomNumberService(
        HttpClient httpClient,
        IOptions<ExternalApiSettings> externalApiSettings,
        ILogger<HttpRandomNumberService> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _settings = externalApiSettings?.Value?.RandomNumberService ?? throw new ArgumentNullException(nameof(externalApiSettings));
    }

    public async Task<int> GetRandomNumberAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Requesting random number from external API");

            var response = await _httpClient.GetAsync("/random", cancellationToken);
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                
                if (int.TryParse(content, out var randomNumber))
                {
                    _logger.LogDebug("Successfully received random number: {RandomNumber}", randomNumber);
                    return randomNumber;
                }
                
                _logger.LogWarning("Failed to parse random number from response: {Content}", content);
            }
            else
            {
                _logger.LogWarning("External API returned non-success status: {StatusCode}", response.StatusCode);
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP request failed when calling random number service");
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, "Request to random number service timed out");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error when calling random number service");
        }

        if (_settings.EnableFallback)
        {
            var fallbackNumber = _fallbackRandom.Next(1, 101);
            _logger.LogInformation("Using fallback random number: {FallbackNumber}", fallbackNumber);
            return fallbackNumber;
        }

        throw new InvalidOperationException("Failed to get random number from external service and fallback is disabled");
    }
}