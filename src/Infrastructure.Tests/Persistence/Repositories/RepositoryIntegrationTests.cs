using FluentAssertions;
using RentalRepairs.Domain.Repositories;
using RentalRepairs.Infrastructure.Persistence;
using RentalRepairs.Infrastructure.Persistence.Repositories;
using RentalRepairs.Infrastructure.Persistence.Configurations;
using Xunit;

namespace RentalRepairs.Infrastructure.Tests.Persistence.Repositories;

public class RepositoryIntegrationTests
{
    [Fact]
    public void Repository_Types_Should_Implement_Correct_Interfaces()
    {
        // Verify repository types implement their interfaces
        var propertyRepoType = typeof(PropertyRepository);
        var tenantRepoType = typeof(TenantRepository);
        var tenantRequestRepoType = typeof(TenantRequestRepository);
        var workerRepoType = typeof(WorkerRepository);

        propertyRepoType.Should().BeAssignableTo<IPropertyRepository>();
        tenantRepoType.Should().BeAssignableTo<ITenantRepository>();
        tenantRequestRepoType.Should().BeAssignableTo<ITenantRequestRepository>();
        workerRepoType.Should().BeAssignableTo<IWorkerRepository>();
    }

    [Fact]
    public void Repository_Types_Should_Have_Required_Methods()
    {
        // Test PropertyRepository methods
        var propertyRepoMethods = typeof(PropertyRepository).GetMethods().Select(m => m.Name).ToList();
        propertyRepoMethods.Should().Contain("GetByIdAsync");
        propertyRepoMethods.Should().Contain("GetByCodeAsync");
        propertyRepoMethods.Should().Contain("GetAllAsync");
        propertyRepoMethods.Should().Contain("AddAsync");
        propertyRepoMethods.Should().Contain("Update");
        propertyRepoMethods.Should().Contain("Remove");
        propertyRepoMethods.Should().Contain("SaveChangesAsync");
        propertyRepoMethods.Should().Contain("ExistsAsync");

        // Test TenantRequestRepository methods
        var tenantRequestRepoMethods = typeof(TenantRequestRepository).GetMethods().Select(m => m.Name).ToList();
        tenantRequestRepoMethods.Should().Contain("GetByIdAsync");
        tenantRequestRepoMethods.Should().Contain("GetByStatusAsync");
        tenantRequestRepoMethods.Should().Contain("GetByUrgencyLevelAsync");
        tenantRequestRepoMethods.Should().Contain("GetPendingRequestsAsync");
        tenantRequestRepoMethods.Should().Contain("GetOverdueRequestsAsync");
        tenantRequestRepoMethods.Should().Contain("CountByStatusAsync");
    }

    [Fact]
    public void Repository_Types_Should_Be_In_Correct_Namespace()
    {
        var propertyRepoType = typeof(PropertyRepository);
        var tenantRepoType = typeof(TenantRepository);
        var tenantRequestRepoType = typeof(TenantRequestRepository);
        var workerRepoType = typeof(WorkerRepository);

        propertyRepoType.Namespace.Should().Be("RentalRepairs.Infrastructure.Persistence.Repositories");
        tenantRepoType.Namespace.Should().Be("RentalRepairs.Infrastructure.Persistence.Repositories");
        tenantRequestRepoType.Namespace.Should().Be("RentalRepairs.Infrastructure.Persistence.Repositories");
        workerRepoType.Namespace.Should().Be("RentalRepairs.Infrastructure.Persistence.Repositories");
    }

    [Fact]
    public void ApplicationDbContext_Type_Should_Be_Correct()
    {
        var dbContextType = typeof(ApplicationDbContext);
        
        dbContextType.Should().NotBeNull();
        dbContextType.Namespace.Should().Be("RentalRepairs.Infrastructure.Persistence");
        dbContextType.Should().BeAssignableTo<Microsoft.EntityFrameworkCore.DbContext>();
        dbContextType.Should().BeAssignableTo<Application.Common.Interfaces.IApplicationDbContext>();
    }

    [Fact]
    public void Entity_Configurations_Should_Exist()
    {
        var propertyConfigType = typeof(PropertyConfiguration);
        var tenantConfigType = typeof(TenantConfiguration);
        var tenantRequestConfigType = typeof(TenantRequestConfiguration);
        var workerConfigType = typeof(WorkerConfiguration);

        propertyConfigType.Should().NotBeNull();
        tenantConfigType.Should().NotBeNull();
        tenantRequestConfigType.Should().NotBeNull();
        workerConfigType.Should().NotBeNull();

        propertyConfigType.Should().BeAssignableTo<Microsoft.EntityFrameworkCore.IEntityTypeConfiguration<Domain.Entities.Property>>();
        tenantConfigType.Should().BeAssignableTo<Microsoft.EntityFrameworkCore.IEntityTypeConfiguration<Domain.Entities.Tenant>>();
        tenantRequestConfigType.Should().BeAssignableTo<Microsoft.EntityFrameworkCore.IEntityTypeConfiguration<Domain.Entities.TenantRequest>>();
        workerConfigType.Should().BeAssignableTo<Microsoft.EntityFrameworkCore.IEntityTypeConfiguration<Domain.Entities.Worker>>();
    }
}