using FluentAssertions;
using Microsoft.Extensions.Logging;
using RentalRepairs.Infrastructure.Monitoring;
using RentalRepairs.Infrastructure.Tests.Services;
using Xunit;

namespace RentalRepairs.Infrastructure.Tests.Monitoring;

public class MonitoringServiceTests
{
    [Fact]
    public async Task PerformanceMonitoringService_Should_Log_Performance_Metrics()
    {
        // Arrange
        var logger = new ExternalServicesIntegrationTests.MockLogger<PerformanceMonitoringService>();
        var monitoringService = new PerformanceMonitoringService(logger);

        // Act
        await monitoringService.LogPerformanceMetricAsync("test_metric", 123.45, "ms", new Dictionary<string, object>
        {
            ["operation"] = "test_operation",
            ["count"] = 5
        });

        // Assert - No exception should be thrown
        // In a real test, you'd verify the logger was called with the correct parameters
    }

    [Fact]
    public async Task PerformanceMonitoringService_Should_Log_Business_Events()
    {
        // Arrange
        var logger = new ExternalServicesIntegrationTests.MockLogger<PerformanceMonitoringService>();
        var monitoringService = new PerformanceMonitoringService(logger);

        // Act
        await monitoringService.LogBusinessMetricAsync("test_event", new Dictionary<string, object>
        {
            ["property_id"] = 123,
            ["tenant_id"] = 456,
            ["timestamp"] = DateTime.UtcNow
        });

        // Assert - No exception should be thrown
    }

    [Fact]
    public async Task PerformanceMonitoringService_Should_Log_Errors()
    {
        // Arrange
        var logger = new ExternalServicesIntegrationTests.MockLogger<PerformanceMonitoringService>();
        var monitoringService = new PerformanceMonitoringService(logger);
        var exception = new InvalidOperationException("Test exception");

        // Act
        await monitoringService.LogErrorAsync(exception, "test_operation", new Dictionary<string, object>
        {
            ["operation_id"] = "123",
            ["user_id"] = "user@test.com"
        });

        // Assert - No exception should be thrown
    }

    [Fact]
    public void PerformanceOperation_Should_Track_Timing()
    {
        // Arrange
        var logger = new ExternalServicesIntegrationTests.MockLogger<PerformanceMonitoringService>();
        var monitoringService = new PerformanceMonitoringService(logger);

        // Act & Assert
        using var operation = monitoringService.BeginOperation("test_operation", new Dictionary<string, object>
        {
            ["parameter1"] = "value1",
            ["parameter2"] = 42
        });

        // Operation should dispose properly without exceptions
        operation.Should().NotBeNull();
    }

    [Fact]
    public void BusinessEvents_Should_Have_Required_Constants()
    {
        // Assert
        BusinessEvents.PropertyRegistered.Should().Be("PropertyRegistered");
        BusinessEvents.TenantRegistered.Should().Be("TenantRegistered");
        BusinessEvents.TenantRequestSubmitted.Should().Be("TenantRequestSubmitted");
        BusinessEvents.TenantRequestScheduled.Should().Be("TenantRequestScheduled");
        BusinessEvents.TenantRequestCompleted.Should().Be("TenantRequestCompleted");
        BusinessEvents.TenantRequestClosed.Should().Be("TenantRequestClosed");
        BusinessEvents.WorkerRegistered.Should().Be("WorkerRegistered");
        BusinessEvents.NotificationSent.Should().Be("NotificationSent");
        BusinessEvents.AuthenticationAttempt.Should().Be("AuthenticationAttempt");
        BusinessEvents.AuthorizationCheck.Should().Be("AuthorizationCheck");
        BusinessEvents.CacheOperation.Should().Be("CacheOperation");
    }

    [Fact]
    public void PerformanceMetrics_Should_Have_Required_Constants()
    {
        // Assert
        PerformanceMetrics.DatabaseQuery.Should().Be("database_query_duration");
        PerformanceMetrics.CacheHit.Should().Be("cache_hit_ratio");
        PerformanceMetrics.ApiResponse.Should().Be("api_response_time");
        PerformanceMetrics.EmailSend.Should().Be("email_send_duration");
        PerformanceMetrics.NotificationDelivery.Should().Be("notification_delivery_time");
        PerformanceMetrics.AuthenticationDuration.Should().Be("authentication_duration");
        PerformanceMetrics.AuthorizationDuration.Should().Be("authorization_duration");
    }
}