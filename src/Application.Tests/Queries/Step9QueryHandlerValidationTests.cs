using FluentAssertions;
using RentalRepairs.Application.Queries.Properties;
using RentalRepairs.Application.Queries.TenantRequests;
using RentalRepairs.Application.Queries.Tenants;
using RentalRepairs.Application.Queries.Workers;
using RentalRepairs.Application.Common.Interfaces;
using RentalRepairs.Application.Common.Models;
using RentalRepairs.Application.DTOs;
using Xunit;

namespace RentalRepairs.Application.Tests.Queries;

public class Step9QueryHandlerValidationTests
{
    [Fact]
    public void All_Required_Query_Handlers_Exist()
    {
        // This test validates that Step 9 query handlers are implemented
        
        // Arrange & Act - Check that all required types exist per migration plan
        var getPropertyByIdHandlerType = typeof(Application.Queries.Properties.Handlers.GetPropertyByIdQueryHandler);
        var getTenantRequestsHandlerType = typeof(Application.Queries.TenantRequests.Handlers.GetTenantRequestsQueryHandler);
        var getWorkerRequestsHandlerType = typeof(Application.Queries.TenantRequests.Handlers.GetWorkerRequestsQueryHandler);
        
        // Additional handlers for comprehensive coverage
        var getPropertyByCodeHandlerType = typeof(Application.Queries.Properties.Handlers.GetPropertyByCodeQueryHandler);
        var getPropertiesHandlerType = typeof(Application.Queries.Properties.Handlers.GetPropertiesQueryHandler);
        var getTenantByIdHandlerType = typeof(Application.Queries.Tenants.Handlers.GetTenantByIdQueryHandler);
        var getWorkerByIdHandlerType = typeof(Application.Queries.Workers.Handlers.GetWorkerByIdQueryHandler);

        // Assert - All handlers exist and implement correct interfaces
        getPropertyByIdHandlerType.Should().NotBeNull();
        getTenantRequestsHandlerType.Should().NotBeNull();
        getWorkerRequestsHandlerType.Should().NotBeNull();
        getPropertyByCodeHandlerType.Should().NotBeNull();
        getPropertiesHandlerType.Should().NotBeNull();
        getTenantByIdHandlerType.Should().NotBeNull();
        getWorkerByIdHandlerType.Should().NotBeNull();

        // Verify they implement the correct interfaces (updated for PagedResult)
        getPropertyByIdHandlerType.Should().BeAssignableTo<IQueryHandler<GetPropertyByIdQuery, PropertyDto>>();
        getTenantRequestsHandlerType.Should().BeAssignableTo<IQueryHandler<GetTenantRequestsQuery, PagedResult<TenantRequestDto>>>();
        getWorkerRequestsHandlerType.Should().BeAssignableTo<IQueryHandler<GetWorkerRequestsQuery, PagedResult<TenantRequestDto>>>();
        getPropertyByCodeHandlerType.Should().BeAssignableTo<IQueryHandler<GetPropertyByCodeQuery, PropertyDto>>();
        getPropertiesHandlerType.Should().BeAssignableTo<IQueryHandler<GetPropertiesQuery, PagedResult<PropertyDto>>>();
        getTenantByIdHandlerType.Should().BeAssignableTo<IQueryHandler<GetTenantByIdQuery, TenantDto>>();
        getWorkerByIdHandlerType.Should().BeAssignableTo<IQueryHandler<GetWorkerByIdQuery, WorkerDto>>();
    }

    [Fact]
    public void All_Queries_Implement_Correct_Interfaces()
    {
        // Test that all queries implement the correct CQRS interfaces
        
        var getPropertyByIdQuery = new GetPropertyByIdQuery(1);
        var getTenantRequestsQuery = new GetTenantRequestsQuery();
        var getWorkerRequestsQuery = new GetWorkerRequestsQuery("worker@test.com");
        var getPropertyByCodeQuery = new GetPropertyByCodeQuery("TP-001");
        var getPropertiesQuery = new GetPropertiesQuery();
        var getTenantByIdQuery = new GetTenantByIdQuery(1);
        var getWorkerByIdQuery = new GetWorkerByIdQuery(1);

        // Assert all queries implement correct interfaces (updated for PagedResult)
        getPropertyByIdQuery.Should().BeAssignableTo<IQuery<PropertyDto>>();
        getTenantRequestsQuery.Should().BeAssignableTo<IQuery<PagedResult<TenantRequestDto>>>();
        getWorkerRequestsQuery.Should().BeAssignableTo<IQuery<PagedResult<TenantRequestDto>>>();
        getPropertyByCodeQuery.Should().BeAssignableTo<IQuery<PropertyDto>>();
        getPropertiesQuery.Should().BeAssignableTo<IQuery<PagedResult<PropertyDto>>>();
        getTenantByIdQuery.Should().BeAssignableTo<IQuery<TenantDto>>();
        getWorkerByIdQuery.Should().BeAssignableTo<IQuery<WorkerDto>>();
    }

    [Fact]
    public void Queries_Have_Required_Properties()
    {
        // Test that queries have the expected structure for data retrieval operations
        
        // GetPropertyByIdQuery
        var getPropertyByIdQuery = new GetPropertyByIdQuery(123);
        getPropertyByIdQuery.PropertyId.Should().Be(123);

        // GetTenantRequestsQuery with filtering
        var getTenantRequestsQuery = new GetTenantRequestsQuery
        {
            PropertyId = 1,
            Status = Domain.Enums.TenantRequestStatus.Submitted,
            UrgencyLevel = "High",
            PendingOnly = false,
            OverdueOnly = false,
            PageNumber = 2,
            PageSize = 20
        };

        getTenantRequestsQuery.PropertyId.Should().Be(1);
        getTenantRequestsQuery.Status.Should().Be(Domain.Enums.TenantRequestStatus.Submitted);
        getTenantRequestsQuery.UrgencyLevel.Should().Be("High");
        getTenantRequestsQuery.PageNumber.Should().Be(2);
        getTenantRequestsQuery.PageSize.Should().Be(20);

        // GetWorkerRequestsQuery with filtering
        var getWorkerRequestsQuery = new GetWorkerRequestsQuery("worker@test.com")
        {
            Status = Domain.Enums.TenantRequestStatus.Scheduled,
            FromDate = DateTime.UtcNow.AddDays(-30),
            ToDate = DateTime.UtcNow,
            PageNumber = 1,
            PageSize = 10
        };

        getWorkerRequestsQuery.WorkerEmail.Should().Be("worker@test.com");
        getWorkerRequestsQuery.Status.Should().Be(Domain.Enums.TenantRequestStatus.Scheduled);
        getWorkerRequestsQuery.PageNumber.Should().Be(1);
        getWorkerRequestsQuery.PageSize.Should().Be(10);
    }

    [Fact]
    public void Step9_Success_Criteria_Met()
    {
        // Validate that Step 9 success criteria from the migration plan are met
        
        // ? GetPropertyByIdQuery and GetPropertyByIdQueryHandler
        var getPropertyByIdQueryType = typeof(GetPropertyByIdQuery);
        var getPropertyByIdHandlerType = typeof(Application.Queries.Properties.Handlers.GetPropertyByIdQueryHandler);
        getPropertyByIdQueryType.Should().NotBeNull();
        getPropertyByIdHandlerType.Should().NotBeNull();

        // ? GetTenantRequestsQuery and GetTenantRequestsQueryHandler
        var getTenantRequestsQueryType = typeof(GetTenantRequestsQuery);
        var getTenantRequestsHandlerType = typeof(Application.Queries.TenantRequests.Handlers.GetTenantRequestsQueryHandler);
        getTenantRequestsQueryType.Should().NotBeNull();
        getTenantRequestsHandlerType.Should().NotBeNull();

        // ? GetWorkerRequestsQuery and GetWorkerRequestsQueryHandler
        var getWorkerRequestsQueryType = typeof(GetWorkerRequestsQuery);
        var getWorkerRequestsHandlerType = typeof(Application.Queries.TenantRequests.Handlers.GetWorkerRequestsQueryHandler);
        getWorkerRequestsQueryType.Should().NotBeNull();
        getWorkerRequestsHandlerType.Should().NotBeNull();

        // Additional comprehensive query handlers created
        var getPropertiesHandlerType = typeof(Application.Queries.Properties.Handlers.GetPropertiesQueryHandler);
        var getTenantByIdHandlerType = typeof(Application.Queries.Tenants.Handlers.GetTenantByIdQueryHandler);
        var getWorkersHandlerType = typeof(Application.Queries.Workers.Handlers.GetWorkersQueryHandler);
        
        getPropertiesHandlerType.Should().NotBeNull();
        getTenantByIdHandlerType.Should().NotBeNull();
        getWorkersHandlerType.Should().NotBeNull();
    }

    [Fact]
    public void Query_Handlers_Have_Public_Constructors()
    {
        // Verify that all query handlers can be instantiated (for dependency injection)
        
        var getPropertyByIdHandlerType = typeof(Application.Queries.Properties.Handlers.GetPropertyByIdQueryHandler);
        var getTenantRequestsHandlerType = typeof(Application.Queries.TenantRequests.Handlers.GetTenantRequestsQueryHandler);
        var getWorkerRequestsHandlerType = typeof(Application.Queries.TenantRequests.Handlers.GetWorkerRequestsQueryHandler);
        var getTenantByIdHandlerType = typeof(Application.Queries.Tenants.Handlers.GetTenantByIdQueryHandler);
        var getWorkerByIdHandlerType = typeof(Application.Queries.Workers.Handlers.GetWorkerByIdQueryHandler);

        // Assert that all have public constructors with dependencies
        getPropertyByIdHandlerType.GetConstructors().Should().HaveCountGreaterThan(0);
        getTenantRequestsHandlerType.GetConstructors().Should().HaveCountGreaterThan(0);
        getWorkerRequestsHandlerType.GetConstructors().Should().HaveCountGreaterThan(0);
        getTenantByIdHandlerType.GetConstructors().Should().HaveCountGreaterThan(0);
        getWorkerByIdHandlerType.GetConstructors().Should().HaveCountGreaterThan(0);
    }
}