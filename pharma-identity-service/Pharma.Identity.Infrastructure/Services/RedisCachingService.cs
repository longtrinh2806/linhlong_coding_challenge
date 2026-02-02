using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Pharma.Identity.Application.Common.Abstractions;
using Pharma.Identity.Infrastructure.Configurations;

namespace Pharma.Identity.Infrastructure.Services;

public class RedisCachingService(IDistributedCache distributedCache, CacheConfiguration cacheConfiguration) : ICachingService
{
    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var cachedData = await distributedCache.GetStringAsync(key, cancellationToken);

        return cachedData == null ? default : JsonSerializer.Deserialize<T>(cachedData, _jsonSerializerOptions);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null,
        CancellationToken cancellationToken = default)
    {
        var serializedData = JsonSerializer.Serialize(value, _jsonSerializerOptions);

        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration ?? cacheConfiguration.DefaultExpiration
        };

        await distributedCache.SetStringAsync(key, serializedData, options, cancellationToken);
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        await distributedCache.RemoveAsync(key, cancellationToken);
    }

    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        var cachedData = await distributedCache.GetStringAsync(key, cancellationToken);

        return !string.IsNullOrEmpty(cachedData);
    }
}