using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RentalRepairs.Infrastructure.Caching;
using RentalRepairs.Infrastructure.Tests.Services;
using Xunit;

namespace RentalRepairs.Infrastructure.Tests.Caching;

public class CachingServiceTests
{
    [Fact]
    public async Task MemoryCacheService_Should_Store_And_Retrieve_Values()
    {
        // Arrange
        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        var settings = Options.Create(new CacheSettings { EnableCaching = true });
        var logger = new ExternalServicesIntegrationTests.MockLogger<MemoryCacheService>();
        var cacheService = new MemoryCacheService(memoryCache, settings, logger);

        var testObject = new TestCacheObject { Id = 1, Name = "Test" };

        // Act
        await cacheService.SetAsync("test-key", testObject);
        var retrieved = await cacheService.GetAsync<TestCacheObject>("test-key");

        // Assert
        retrieved.Should().NotBeNull();
        retrieved!.Id.Should().Be(1);
        retrieved.Name.Should().Be("Test");
    }

    [Fact]
    public async Task MemoryCacheService_Should_Return_Null_For_Missing_Key()
    {
        // Arrange
        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        var settings = Options.Create(new CacheSettings { EnableCaching = true });
        var logger = new ExternalServicesIntegrationTests.MockLogger<MemoryCacheService>();
        var cacheService = new MemoryCacheService(memoryCache, settings, logger);

        // Act
        var retrieved = await cacheService.GetAsync<TestCacheObject>("missing-key");

        // Assert
        retrieved.Should().BeNull();
    }

    [Fact]
    public async Task MemoryCacheService_Should_Remove_Values()
    {
        // Arrange
        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        var settings = Options.Create(new CacheSettings { EnableCaching = true });
        var logger = new ExternalServicesIntegrationTests.MockLogger<MemoryCacheService>();
        var cacheService = new MemoryCacheService(memoryCache, settings, logger);

        var testObject = new TestCacheObject { Id = 1, Name = "Test" };

        // Act
        await cacheService.SetAsync("test-key", testObject);
        await cacheService.RemoveAsync("test-key");
        var retrieved = await cacheService.GetAsync<TestCacheObject>("test-key");

        // Assert
        retrieved.Should().BeNull();
    }

    [Fact]
    public async Task MemoryCacheService_Should_Check_Existence()
    {
        // Arrange
        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        var settings = Options.Create(new CacheSettings { EnableCaching = true });
        var logger = new ExternalServicesIntegrationTests.MockLogger<MemoryCacheService>();
        var cacheService = new MemoryCacheService(memoryCache, settings, logger);

        var testObject = new TestCacheObject { Id = 1, Name = "Test" };

        // Act & Assert
        var existsBeforeSet = await cacheService.ExistsAsync("test-key");
        existsBeforeSet.Should().BeFalse();

        await cacheService.SetAsync("test-key", testObject);
        var existsAfterSet = await cacheService.ExistsAsync("test-key");
        existsAfterSet.Should().BeTrue();
    }

    [Fact]
    public async Task NullCacheService_Should_Always_Return_Null()
    {
        // Arrange
        var logger = new ExternalServicesIntegrationTests.MockLogger<NullCacheService>();
        var cacheService = new NullCacheService(logger);

        var testObject = new TestCacheObject { Id = 1, Name = "Test" };

        // Act
        await cacheService.SetAsync("test-key", testObject);
        var retrieved = await cacheService.GetAsync<TestCacheObject>("test-key");

        // Assert
        retrieved.Should().BeNull();
    }

    [Fact]
    public void CacheKeys_Should_Generate_Correct_Keys()
    {
        // Act & Assert
        CacheKeys.Property(123).Should().Be("rental_repairs:property:123");
        CacheKeys.PropertyByCode("PROP001").Should().Be("rental_repairs:property:code:PROP001");
        CacheKeys.PropertiesByCity("New York").Should().Be("rental_repairs:properties:city:New York");
        CacheKeys.Tenant(456).Should().Be("rental_repairs:tenant:456");
        CacheKeys.TenantsByProperty(123).Should().Be("rental_repairs:tenants:property:123");
        CacheKeys.TenantByPropertyUnit(123, "101").Should().Be("rental_repairs:tenant:property:123:unit:101");
        CacheKeys.TenantRequest(789).Should().Be("rental_repairs:request:789");
        CacheKeys.TenantRequestsByStatus("submitted").Should().Be("rental_repairs:requests:status:submitted");
        CacheKeys.Worker(111).Should().Be("rental_repairs:worker:111");
        CacheKeys.WorkerByEmail("worker@test.com").Should().Be("rental_repairs:worker:email:worker@test.com");
    }

    [Fact]
    public void CacheSettings_Should_Have_Default_Values()
    {
        // Arrange & Act
        var settings = new CacheSettings();

        // Assert
        settings.EnableCaching.Should().BeTrue();
        settings.Provider.Should().Be("Memory");
        settings.DefaultExpiration.Should().Be(TimeSpan.FromMinutes(30));
        settings.ShortExpiration.Should().Be(TimeSpan.FromMinutes(5));
        settings.LongExpiration.Should().Be(TimeSpan.FromHours(2));
        settings.MaxCacheSize.Should().Be(1000);
        settings.EnableCacheMetrics.Should().BeTrue();
    }

    private class TestCacheObject
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}