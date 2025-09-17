using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace RentalRepairs.Infrastructure.Caching;

/// <summary>
/// No-operation cache service for when caching is disabled
/// </summary>
public class NullCacheService : ICacheService
{
    private readonly ILogger<NullCacheService> _logger;

    public NullCacheService(ILogger<NullCacheService> logger)
    {
        _logger = logger;
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
    {
        await Task.CompletedTask;
        return null;
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default) where T : class
    {
        await Task.CompletedTask;
        // No-op
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;
        // No-op
    }

    public async Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;
        // No-op
    }

    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;
        return false;
    }

    public async Task ClearAsync(CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;
        // No-op
    }
}

/// <summary>
/// Cached repository decorator for Property operations
/// </summary>
public class CachedPropertyService
{
    private readonly ICacheService _cacheService;
    private readonly ILogger<CachedPropertyService> _logger;
    private readonly CacheSettings _settings;

    public CachedPropertyService(
        ICacheService cacheService,
        IOptions<CacheSettings> settings,
        ILogger<CachedPropertyService> logger)
    {
        _cacheService = cacheService;
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task<T?> GetOrSetAsync<T>(string cacheKey, Func<Task<T?>> getItem, TimeSpan? expiration = null, CancellationToken cancellationToken = default) where T : class
    {
        try
        {
            // Try to get from cache first
            var cachedItem = await _cacheService.GetAsync<T>(cacheKey, cancellationToken);
            if (cachedItem != null)
            {
                return cachedItem;
            }

            // Get from data source
            var item = await getItem();
            if (item != null)
            {
                // Cache the result
                await _cacheService.SetAsync(cacheKey, item, expiration ?? _settings.DefaultExpiration, cancellationToken);
            }

            return item;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in cached operation for key: {CacheKey}", cacheKey);
            // Fallback to direct data access
            return await getItem();
        }
    }

    public async Task InvalidatePropertyCacheAsync(int propertyId, CancellationToken cancellationToken = default)
    {
        var keysToInvalidate = new[]
        {
            CacheKeys.Property(propertyId),
            CacheKeys.TenantsByProperty(propertyId),
            CacheKeys.TenantRequestsByProperty(propertyId)
        };

        foreach (var key in keysToInvalidate)
        {
            await _cacheService.RemoveAsync(key, cancellationToken);
        }

        _logger.LogDebug("Invalidated property cache for property {PropertyId}", propertyId);
    }

    public async Task InvalidateTenantCacheAsync(int tenantId, int propertyId, CancellationToken cancellationToken = default)
    {
        var keysToInvalidate = new[]
        {
            CacheKeys.Tenant(tenantId),
            CacheKeys.TenantsByProperty(propertyId),
            CacheKeys.TenantRequestsByTenant(tenantId)
        };

        foreach (var key in keysToInvalidate)
        {
            await _cacheService.RemoveAsync(key, cancellationToken);
        }

        _logger.LogDebug("Invalidated tenant cache for tenant {TenantId}", tenantId);
    }

    public async Task InvalidateRequestCacheAsync(int requestId, int tenantId, CancellationToken cancellationToken = default)
    {
        var keysToInvalidate = new[]
        {
            CacheKeys.TenantRequest(requestId),
            CacheKeys.TenantRequestsByTenant(tenantId)
        };

        foreach (var key in keysToInvalidate)
        {
            await _cacheService.RemoveAsync(key, cancellationToken);
        }

        // Also remove status-based caches
        await _cacheService.RemoveByPatternAsync(CacheKeys.RequestPattern(), cancellationToken);

        _logger.LogDebug("Invalidated request cache for request {RequestId}", requestId);
    }
}