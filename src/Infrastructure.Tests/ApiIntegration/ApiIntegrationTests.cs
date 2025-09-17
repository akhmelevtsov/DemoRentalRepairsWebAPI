using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RentalRepairs.Infrastructure.ApiIntegration;
using RentalRepairs.Infrastructure.Configuration;
using RentalRepairs.Infrastructure.Tests.Services;
using Xunit;

namespace RentalRepairs.Infrastructure.Tests.ApiIntegration;

public class ApiIntegrationTests
{
    [Fact]
    public async Task WorkerSchedulingApiClient_Should_Return_Mock_Availability_When_Disabled()
    {
        // Arrange
        var mockApiClient = new MockExternalApiClient();
        var settings = Options.Create(new ExternalServicesSettings
        {
            ApiIntegrations = new ApiIntegrationSettings
            {
                EnableWorkerSchedulingApi = false
            }
        });
        var logger = new ExternalServicesIntegrationTests.MockLogger<WorkerSchedulingApiClient>();
        var apiClient = new WorkerSchedulingApiClient(mockApiClient, settings, logger);

        // Act
        var result = await apiClient.GetWorkerAvailabilityAsync("worker@test.com", DateTime.Today);

        // Assert
        result.Should().NotBeNull();
        result!.WorkerEmail.Should().Be("worker@test.com");
        result.Date.Should().Be(DateTime.Today);
        result.IsAvailable.Should().BeTrue();
        result.AvailableSlots.Should().HaveCount(2);
    }

    [Fact]
    public async Task WorkerSchedulingApiClient_Should_Return_Mock_Schedule_Response_When_Disabled()
    {
        // Arrange
        var mockApiClient = new MockExternalApiClient();
        var settings = Options.Create(new ExternalServicesSettings
        {
            ApiIntegrations = new ApiIntegrationSettings
            {
                EnableWorkerSchedulingApi = false
            }
        });
        var logger = new ExternalServicesIntegrationTests.MockLogger<WorkerSchedulingApiClient>();
        var apiClient = new WorkerSchedulingApiClient(mockApiClient, settings, logger);

        var scheduleRequest = new ScheduleWorkRequest
        {
            WorkerEmail = "worker@test.com",
            WorkOrderId = "WO-123",
            ScheduledDate = DateTime.Today.AddDays(1),
            EstimatedDuration = TimeSpan.FromHours(2),
            WorkType = "Plumbing",
            PropertyAddress = "123 Main St",
            ContactEmail = "tenant@test.com",
            Description = "Fix leaky faucet",
            UrgencyLevel = "Normal"
        };

        // Act
        var result = await apiClient.ScheduleWorkAsync(scheduleRequest);

        // Assert
        result.Should().NotBeNull();
        result!.IsSuccess.Should().BeTrue();
        result.WorkOrderId.Should().Be("WO-123");
        result.ScheduledDate.Should().Be(DateTime.Today.AddDays(1));
        result.ConfirmationNumber.Should().NotBeEmpty();
    }

    [Fact]
    public async Task WorkerSchedulingApiClient_Should_Return_True_For_Cancel_When_Disabled()
    {
        // Arrange
        var mockApiClient = new MockExternalApiClient();
        var settings = Options.Create(new ExternalServicesSettings
        {
            ApiIntegrations = new ApiIntegrationSettings
            {
                EnableWorkerSchedulingApi = false
            }
        });
        var logger = new ExternalServicesIntegrationTests.MockLogger<WorkerSchedulingApiClient>();
        var apiClient = new WorkerSchedulingApiClient(mockApiClient, settings, logger);

        // Act
        var result = await apiClient.CancelScheduledWorkAsync("WO-123");

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task WorkerSchedulingApiClient_Should_Return_Mock_Schedule_When_Disabled()
    {
        // Arrange
        var mockApiClient = new MockExternalApiClient();
        var settings = Options.Create(new ExternalServicesSettings
        {
            ApiIntegrations = new ApiIntegrationSettings
            {
                EnableWorkerSchedulingApi = false
            }
        });
        var logger = new ExternalServicesIntegrationTests.MockLogger<WorkerSchedulingApiClient>();
        var apiClient = new WorkerSchedulingApiClient(mockApiClient, settings, logger);

        var startDate = DateTime.Today;
        var endDate = DateTime.Today.AddDays(7);

        // Act
        var result = await apiClient.GetWorkerScheduleAsync("worker@test.com", startDate, endDate);

        // Assert
        result.Should().NotBeNull();
        result!.WorkerEmail.Should().Be("worker@test.com");
        result.StartDate.Should().Be(startDate);
        result.EndDate.Should().Be(endDate);
        result.ScheduledWork.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public void ScheduleWorkRequest_Should_Have_Required_Properties()
    {
        // Arrange & Act
        var request = new ScheduleWorkRequest
        {
            WorkerEmail = "worker@test.com",
            WorkOrderId = "WO-123",
            ScheduledDate = DateTime.Today,
            EstimatedDuration = TimeSpan.FromHours(2),
            WorkType = "Plumbing",
            PropertyAddress = "123 Main St",
            ContactEmail = "tenant@test.com",
            Description = "Fix issue",
            UrgencyLevel = "High"
        };

        // Assert
        request.WorkerEmail.Should().Be("worker@test.com");
        request.WorkOrderId.Should().Be("WO-123");
        request.ScheduledDate.Should().Be(DateTime.Today);
        request.EstimatedDuration.Should().Be(TimeSpan.FromHours(2));
        request.WorkType.Should().Be("Plumbing");
        request.PropertyAddress.Should().Be("123 Main St");
        request.ContactEmail.Should().Be("tenant@test.com");
        request.Description.Should().Be("Fix issue");
        request.UrgencyLevel.Should().Be("High");
    }

    [Fact]
    public void TimeSlot_Should_Have_Required_Properties()
    {
        // Arrange & Act
        var timeSlot = new TimeSlot
        {
            StartTime = new TimeSpan(9, 0, 0),
            EndTime = new TimeSpan(12, 0, 0),
            IsAvailable = true
        };

        // Assert
        timeSlot.StartTime.Should().Be(new TimeSpan(9, 0, 0));
        timeSlot.EndTime.Should().Be(new TimeSpan(12, 0, 0));
        timeSlot.IsAvailable.Should().BeTrue();
    }

    // Mock implementation for testing
    private class MockExternalApiClient : IExternalApiClient
    {
        public async Task<T?> GetAsync<T>(string endpoint, CancellationToken cancellationToken = default) where T : class
        {
            await Task.CompletedTask;
            return null;
        }

        public async Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest request, CancellationToken cancellationToken = default)
            where TRequest : class where TResponse : class
        {
            await Task.CompletedTask;
            return null;
        }

        public async Task<TResponse?> PutAsync<TRequest, TResponse>(string endpoint, TRequest request, CancellationToken cancellationToken = default)
            where TRequest : class where TResponse : class
        {
            await Task.CompletedTask;
            return null;
        }

        public async Task<bool> DeleteAsync(string endpoint, CancellationToken cancellationToken = default)
        {
            await Task.CompletedTask;
            return true;
        }
    }
}