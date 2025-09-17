namespace RentalRepairs.Infrastructure.Caching;

/// <summary>
/// Cache service interface for application data caching
/// </summary>
public interface ICacheService
{
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class;
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default) where T : class;
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);
    Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default);
    Task ClearAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Cache key generator for consistent cache key management
/// </summary>
public static class CacheKeys
{
    private const string PREFIX = "rental_repairs";
    
    // Property cache keys
    public static string Property(int propertyId) => $"{PREFIX}:property:{propertyId}";
    public static string PropertyByCode(string code) => $"{PREFIX}:property:code:{code}";
    public static string PropertiesByCity(string city) => $"{PREFIX}:properties:city:{city}";
    
    // Tenant cache keys
    public static string Tenant(int tenantId) => $"{PREFIX}:tenant:{tenantId}";
    public static string TenantsByProperty(int propertyId) => $"{PREFIX}:tenants:property:{propertyId}";
    public static string TenantByPropertyUnit(int propertyId, string unitNumber) => 
        $"{PREFIX}:tenant:property:{propertyId}:unit:{unitNumber}";
    
    // Tenant request cache keys
    public static string TenantRequest(int requestId) => $"{PREFIX}:request:{requestId}";
    public static string TenantRequestsByStatus(string status) => $"{PREFIX}:requests:status:{status}";
    public static string TenantRequestsByProperty(int propertyId) => $"{PREFIX}:requests:property:{propertyId}";
    public static string TenantRequestsByTenant(int tenantId) => $"{PREFIX}:requests:tenant:{tenantId}";
    
    // Worker cache keys
    public static string Worker(int workerId) => $"{PREFIX}:worker:{workerId}";
    public static string WorkerByEmail(string email) => $"{PREFIX}:worker:email:{email}";
    public static string WorkersBySpecialization(string specialization) => $"{PREFIX}:workers:spec:{specialization}";
    public static string ActiveWorkers() => $"{PREFIX}:workers:active";
    
    // Pattern keys for bulk operations
    public static string PropertyPattern() => $"{PREFIX}:property:*";
    public static string TenantPattern() => $"{PREFIX}:tenant:*";
    public static string RequestPattern() => $"{PREFIX}:request:*";
    public static string WorkerPattern() => $"{PREFIX}:worker:*";
}

/// <summary>
/// Cache configuration settings
/// </summary>
public class CacheSettings
{
    public const string SectionName = "CacheSettings";
    
    public bool EnableCaching { get; set; } = true;
    public string Provider { get; set; } = "Memory"; // Memory, Redis, DistributedMemory
    public TimeSpan DefaultExpiration { get; set; } = TimeSpan.FromMinutes(30);
    public TimeSpan ShortExpiration { get; set; } = TimeSpan.FromMinutes(5);
    public TimeSpan LongExpiration { get; set; } = TimeSpan.FromHours(2);
    public int MaxCacheSize { get; set; } = 1000;
    public bool EnableCacheMetrics { get; set; } = true;
    public string? RedisConnectionString { get; set; }
}