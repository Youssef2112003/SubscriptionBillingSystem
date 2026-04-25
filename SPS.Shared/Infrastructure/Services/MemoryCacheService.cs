using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using SPS.Shared.Abstractions;

namespace SPS.Shared.Infrastructure.Services;

public class MemoryCacheService : ICacheService
{
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<MemoryCacheService> _logger;

    public MemoryCacheService(IMemoryCache memoryCache, ILogger<MemoryCacheService> logger)
    {
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            _logger.LogWarning("Attempted to get cache item with empty key.");
            return Task.FromResult(default(T));
        }

        try
        {
            var found = _memoryCache.TryGetValue(key, out T? value);
            if (found)
            {
                _logger.LogDebug("Cache hit for key: {Key}", key);
                return Task.FromResult(value);
            }

            _logger.LogDebug("Cache miss for key: {Key}", key);
            return Task.FromResult(default(T));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving cache item with key: {Key}", key);
            return Task.FromResult(default(T));
        }
    }

    /// <inheritdoc />
    public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            _logger.LogWarning("Attempted to set cache item with empty key.");
            return Task.CompletedTask;
        }

        try
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions();

            if (expiration.HasValue)
            {
                cacheEntryOptions.SetAbsoluteExpiration(expiration.Value);
            }
            else
            {

                cacheEntryOptions.SetAbsoluteExpiration(TimeSpan.FromMinutes(10));
            }


            cacheEntryOptions.RegisterPostEvictionCallback((keyObj, valueObj, reason, state) =>
            {
                _logger.LogDebug("Cache entry {Key} was evicted. Reason: {Reason}", keyObj, reason);
            });

            _memoryCache.Set(key, value, cacheEntryOptions);
            _logger.LogDebug("Cache set for key: {Key} with expiration: {Expiration}", key,
                expiration?.ToString() ?? "default (10 min)");

            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting cache item with key: {Key}", key);
            return Task.CompletedTask;
        }
    }

    /// <inheritdoc />
    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            _logger.LogWarning("Attempted to remove cache item with empty key.");
            return Task.CompletedTask;
        }

        try
        {
            _memoryCache.Remove(key);
            _logger.LogDebug("Cache removed for key: {Key}", key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing cache item with key: {Key}", key);
        }

        return Task.CompletedTask;
    }
}