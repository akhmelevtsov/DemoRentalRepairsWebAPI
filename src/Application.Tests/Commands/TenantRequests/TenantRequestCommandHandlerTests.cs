using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using RentalRepairs.Application.Commands.TenantRequests;
using RentalRepairs.Application.Commands.TenantRequests.Handlers;
using RentalRepairs.Domain.Entities;
using RentalRepairs.Domain.Enums;
using RentalRepairs.Domain.Exceptions;
using RentalRepairs.Domain.Repositories;
using RentalRepairs.Domain.Services;
using RentalRepairs.Domain.ValueObjects;
using Xunit;

namespace RentalRepairs.Application.Tests.Commands.TenantRequests;

public class TenantRequestCommandHandlerTests
{
    [Fact]
    public async Task CreateTenantRequestCommandHandler_Should_Handle_Valid_Request()
    {
        // Arrange
        var tenantRepositoryMock = new Mock<ITenantRepository>();
        var tenantRequestRepositoryMock = new Mock<ITenantRequestRepository>();
        
        // Create a real domain service with proper constructor parameters
        var workerRepositoryMock = new Mock<IWorkerRepository>();
        var domainService = new TenantRequestDomainService(
            tenantRequestRepositoryMock.Object,
            workerRepositoryMock.Object);

        var command = new CreateTenantRequestCommand
        {
            TenantId = 1,
            Title = "Test Request",
            Description = "Test Description",
            UrgencyLevel = "Normal"
        };

        var property = CreateTestProperty();
        var tenant = property.RegisterTenant(
            new PersonContactInfo("Jane", "Smith", "jane@test.com"),
            "101");
        tenant.Id = 1; // Set the tenant ID

        tenantRepositoryMock
            .Setup(x => x.GetByIdAsync(command.TenantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tenant);

        TenantRequest? capturedRequest = null;
        tenantRequestRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<TenantRequest>(), It.IsAny<CancellationToken>()))
            .Callback<TenantRequest, CancellationToken>((request, _) => 
            {
                capturedRequest = request;
                request.Id = 789; // Simulate database setting ID
            })
            .Returns(Task.CompletedTask);

        tenantRequestRepositoryMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new CreateTenantRequestCommandHandler(
            tenantRepositoryMock.Object,
            tenantRequestRepositoryMock.Object,
            domainService);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(789);
        tenantRepositoryMock.Verify(x => x.GetByIdAsync(command.TenantId, It.IsAny<CancellationToken>()), Times.Once);
        tenantRequestRepositoryMock.Verify(x => x.AddAsync(It.IsAny<TenantRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        tenantRequestRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        
        // Verify request properties
        capturedRequest.Should().NotBeNull();
        capturedRequest!.Title.Should().Be(command.Title);
        capturedRequest.Description.Should().Be(command.Description);
        capturedRequest.UrgencyLevel.Should().Be(command.UrgencyLevel);
    }

    [Fact]
    public async Task CreateTenantRequestCommandHandler_Should_Throw_When_Tenant_Not_Found()
    {
        // Arrange
        var tenantRepositoryMock = new Mock<ITenantRepository>();
        var tenantRequestRepositoryMock = new Mock<ITenantRequestRepository>();
        
        var workerRepositoryMock = new Mock<IWorkerRepository>();
        var domainService = new TenantRequestDomainService(
            tenantRequestRepositoryMock.Object,
            workerRepositoryMock.Object);

        var command = new CreateTenantRequestCommand
        {
            TenantId = 999,
            Title = "Test Request",
            Description = "Test Description",
            UrgencyLevel = "Normal"
        };

        tenantRepositoryMock
            .Setup(x => x.GetByIdAsync(command.TenantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Tenant?)null);

        var handler = new CreateTenantRequestCommandHandler(
            tenantRepositoryMock.Object,
            tenantRequestRepositoryMock.Object,
            domainService);

        // Act & Assert
        await Assert.ThrowsAsync<TenantRequestDomainException>(
            () => handler.Handle(command, CancellationToken.None));

        tenantRepositoryMock.Verify(x => x.GetByIdAsync(command.TenantId, It.IsAny<CancellationToken>()), Times.Once);
        tenantRequestRepositoryMock.Verify(x => x.AddAsync(It.IsAny<TenantRequest>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CloseRequestCommandHandler_Should_Close_Request_Successfully()
    {
        // Arrange
        var tenantRequestRepositoryMock = new Mock<ITenantRequestRepository>();
        
        var command = new CloseRequestCommand
        {
            TenantRequestId = 1,
            ClosureNotes = "Request completed successfully"
        };

        var tenantRequest = CreateTestTenantRequest();
        tenantRequest.Id = 1;
        tenantRequest.Submit();
        tenantRequest.Schedule(DateTime.UtcNow.AddDays(1), "worker@test.com", "WO-001");
        tenantRequest.ReportWorkCompleted(true, "Work completed");

        tenantRequestRepositoryMock
            .Setup(x => x.GetByIdAsync(command.TenantRequestId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tenantRequest);

        tenantRequestRepositoryMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new CloseRequestCommandHandler(tenantRequestRepositoryMock.Object);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        tenantRequest.Status.Should().Be(TenantRequestStatus.Closed);
        tenantRequestRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SubmitTenantRequestCommandHandler_Should_Submit_Request_Successfully()
    {
        // Arrange
        var tenantRequestRepositoryMock = new Mock<ITenantRequestRepository>();
        
        var command = new SubmitTenantRequestCommand
        {
            TenantRequestId = 1
        };

        var tenantRequest = CreateTestTenantRequest();
        tenantRequest.Id = 1;

        tenantRequestRepositoryMock
            .Setup(x => x.GetByIdAsync(command.TenantRequestId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tenantRequest);

        tenantRequestRepositoryMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new SubmitTenantRequestCommandHandler(tenantRequestRepositoryMock.Object);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        tenantRequest.Status.Should().Be(TenantRequestStatus.Submitted);
        tenantRequestRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    private static Property CreateTestProperty()
    {
        return new Property(
            "Test Property",
            "TP-001",
            new PropertyAddress("123", "Test St", "Test City", "12345"),
            "555-1234",
            new PersonContactInfo("John", "Doe", "john@test.com"),
            new List<string> { "101", "102" },
            "noreply@test.com");
    }

    private static TenantRequest CreateTestTenantRequest()
    {
        var property = CreateTestProperty();
        var tenant = property.RegisterTenant(
            new PersonContactInfo("Jane", "Smith", "jane@test.com"),
            "101");

        return tenant.CreateRequest("Test Request", "Test Description", "Normal");
    }
}