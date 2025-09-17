using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace RentalRepairs.Infrastructure.Caching;

/// <summary>
/// In-memory cache service implementation
/// </summary>
public class MemoryCacheService : ICacheService
{
    private readonly IMemoryCache _memoryCache;
    private readonly CacheSettings _settings;
    private readonly ILogger<MemoryCacheService> _logger;
    private readonly HashSet<string> _cacheKeys;
    private readonly object _lockObject = new object();

    public MemoryCacheService(
        IMemoryCache memoryCache,
        IOptions<CacheSettings> settings,
        ILogger<MemoryCacheService> logger)
    {
        _memoryCache = memoryCache;
        _settings = settings.Value;
        _logger = logger;
        _cacheKeys = new HashSet<string>();
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            if (!_settings.EnableCaching)
                return null;

            if (_memoryCache.TryGetValue(key, out var cachedValue))
            {
                if (_settings.EnableCacheMetrics)
                    _logger.LogDebug("Cache hit for key: {CacheKey}", key);

                if (cachedValue is string json)
                    return JsonSerializer.Deserialize<T>(json);
                
                return cachedValue as T;
            }

            if (_settings.EnableCacheMetrics)
                _logger.LogDebug("Cache miss for key: {CacheKey}", key);

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving from cache for key: {CacheKey}", key);
            return null;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            if (!_settings.EnableCaching || value == null)
                return;

            var cacheExpiration = expiration ?? _settings.DefaultExpiration;
            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = cacheExpiration,
                Priority = CacheItemPriority.Normal
            };

            // Store as JSON for consistency and to handle complex objects
            var json = JsonSerializer.Serialize(value);
            _memoryCache.Set(key, json, options);

            // Track cache keys for pattern-based operations
            lock (_lockObject)
            {
                _cacheKeys.Add(key);
            }

            if (_settings.EnableCacheMetrics)
                _logger.LogDebug("Cached value for key: {CacheKey} with expiration: {Expiration}", key, cacheExpiration);

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting cache for key: {CacheKey}", key);
        }
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            _memoryCache.Remove(key);

            lock (_lockObject)
            {
                _cacheKeys.Remove(key);
            }

            if (_settings.EnableCacheMetrics)
                _logger.LogDebug("Removed cache key: {CacheKey}", key);

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing cache key: {CacheKey}", key);
        }
    }

    public async Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default)
    {
        try
        {
            var keysToRemove = new List<string>();

            lock (_lockObject)
            {
                var wildcardPattern = pattern.Replace("*", "");
                keysToRemove.AddRange(_cacheKeys.Where(key => key.StartsWith(wildcardPattern)));
            }

            foreach (var key in keysToRemove)
            {
                await RemoveAsync(key, cancellationToken);
            }

            if (_settings.EnableCacheMetrics)
                _logger.LogDebug("Removed {Count} cache keys matching pattern: {Pattern}", keysToRemove.Count, pattern);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing cache keys by pattern: {Pattern}", pattern);
        }
    }

    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        try
        {
            var exists = _memoryCache.TryGetValue(key, out _);
            await Task.CompletedTask;
            return exists;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking cache existence for key: {CacheKey}", key);
            return false;
        }
    }

    public async Task ClearAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var keysToRemove = new List<string>();
            
            lock (_lockObject)
            {
                keysToRemove.AddRange(_cacheKeys);
                _cacheKeys.Clear();
            }

            foreach (var key in keysToRemove)
            {
                _memoryCache.Remove(key);
            }

            if (_settings.EnableCacheMetrics)
                _logger.LogInformation("Cleared {Count} cache entries", keysToRemove.Count);

            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing cache");
        }
    }
}