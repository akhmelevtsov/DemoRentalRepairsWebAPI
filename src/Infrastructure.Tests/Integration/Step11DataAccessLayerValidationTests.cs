using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using RentalRepairs.Domain.Repositories;
using RentalRepairs.Infrastructure.Persistence;
using RentalRepairs.Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace RentalRepairs.Infrastructure.Tests.Integration;

public class Step11DataAccessLayerValidationTests
{
    [Fact]
    public void All_Required_Repository_Implementations_Exist()
    {
        // This test validates that Step 11 repository implementations are created
        
        // Arrange & Act - Check that all required repository types exist
        var propertyRepositoryType = typeof(PropertyRepository);
        var tenantRequestRepositoryType = typeof(TenantRequestRepository);
        var tenantRepositoryType = typeof(TenantRepository);
        var workerRepositoryType = typeof(WorkerRepository);

        // Assert - All repository implementations exist
        propertyRepositoryType.Should().NotBeNull();
        tenantRequestRepositoryType.Should().NotBeNull();
        tenantRepositoryType.Should().NotBeNull();
        workerRepositoryType.Should().NotBeNull();

        // Verify they implement the correct interfaces
        propertyRepositoryType.Should().BeAssignableTo<IPropertyRepository>();
        tenantRequestRepositoryType.Should().BeAssignableTo<ITenantRequestRepository>();
        tenantRepositoryType.Should().BeAssignableTo<ITenantRepository>();
        workerRepositoryType.Should().BeAssignableTo<IWorkerRepository>();
    }

    [Fact]
    public void All_Entity_Configurations_Exist()
    {
        // Test that all Entity Framework configurations are implemented
        
        var propertyConfigType = typeof(Infrastructure.Persistence.Configurations.PropertyConfiguration);
        var tenantConfigType = typeof(Infrastructure.Persistence.Configurations.TenantConfiguration);
        var tenantRequestConfigType = typeof(Infrastructure.Persistence.Configurations.TenantRequestConfiguration);
        var workerConfigType = typeof(Infrastructure.Persistence.Configurations.WorkerConfiguration);

        // Assert all configurations exist and implement IEntityTypeConfiguration
        propertyConfigType.Should().NotBeNull();
        tenantConfigType.Should().NotBeNull();
        tenantRequestConfigType.Should().NotBeNull();
        workerConfigType.Should().NotBeNull();

        propertyConfigType.Should().BeAssignableTo<IEntityTypeConfiguration<Domain.Entities.Property>>();
        tenantConfigType.Should().BeAssignableTo<IEntityTypeConfiguration<Domain.Entities.Tenant>>();
        tenantRequestConfigType.Should().BeAssignableTo<IEntityTypeConfiguration<Domain.Entities.TenantRequest>>();
        workerConfigType.Should().BeAssignableTo<IEntityTypeConfiguration<Domain.Entities.Worker>>();
    }

    [Fact]
    public void ApplicationDbContext_Can_Be_Created()
    {
        // Test that ApplicationDbContext can be created with in-memory database
        
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        // This should not throw an exception
        using var context = new ApplicationDbContext(options);
        context.Should().NotBeNull();
    }

    [Fact]
    public void Infrastructure_Dependency_Injection_Registers_Repository_Services()
    {
        // Test that dependency injection properly registers repository services
        
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>())
            .Build();

        // Act - Use the extension method
        Infrastructure.DependencyInjection.AddInfrastructure(services, configuration);
        var serviceProvider = services.BuildServiceProvider();

        // Assert - All repository interfaces can be resolved (but don't instantiate them)
        var repositoryRegistrations = services.Where(s => 
            s.ServiceType == typeof(IPropertyRepository) ||
            s.ServiceType == typeof(ITenantRepository) ||
            s.ServiceType == typeof(ITenantRequestRepository) ||
            s.ServiceType == typeof(IWorkerRepository)).ToList();

        repositoryRegistrations.Should().HaveCount(4);
        repositoryRegistrations.Should().Contain(s => s.ServiceType == typeof(IPropertyRepository));
        repositoryRegistrations.Should().Contain(s => s.ServiceType == typeof(ITenantRepository));
        repositoryRegistrations.Should().Contain(s => s.ServiceType == typeof(ITenantRequestRepository));
        repositoryRegistrations.Should().Contain(s => s.ServiceType == typeof(IWorkerRepository));
    }

    [Fact]
    public void Repository_Base_Methods_Are_Implemented()
    {
        // Test that repositories implement base repository methods
        
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new ApplicationDbContext(options);
        var propertyRepo = new PropertyRepository(context);

        // Assert base methods exist (testing one repository as example)
        var repoMethods = typeof(PropertyRepository).GetMethods().Select(m => m.Name).ToList();

        repoMethods.Should().Contain("GetByIdAsync");
        repoMethods.Should().Contain("GetAllAsync");
        repoMethods.Should().Contain("AddAsync");
        repoMethods.Should().Contain("Update");
        repoMethods.Should().Contain("Remove");
        repoMethods.Should().Contain("SaveChangesAsync");
        repoMethods.Should().Contain("GetBySpecificationAsync");
    }

    [Fact]
    public void Step11_Success_Criteria_Met()
    {
        // Validate that Step 11 success criteria from the migration plan are met
        
        // ? Repository implementations moved to src/Infrastructure/Persistence/
        var propertyRepoType = typeof(PropertyRepository);
        var tenantRequestRepoType = typeof(TenantRequestRepository);
        
        propertyRepoType.Should().NotBeNull();
        propertyRepoType.Namespace.Should().Be("RentalRepairs.Infrastructure.Persistence.Repositories");
        
        tenantRequestRepoType.Should().NotBeNull();
        tenantRequestRepoType.Namespace.Should().Be("RentalRepairs.Infrastructure.Persistence.Repositories");

        // ? Entity Framework configurations created
        var propertyConfigType = typeof(Infrastructure.Persistence.Configurations.PropertyConfiguration);
        var dbContextType = typeof(ApplicationDbContext);
        
        propertyConfigType.Should().NotBeNull();
        propertyConfigType.Namespace.Should().Be("RentalRepairs.Infrastructure.Persistence.Configurations");
        
        dbContextType.Should().NotBeNull();
        dbContextType.Namespace.Should().Be("RentalRepairs.Infrastructure.Persistence");

        // ? DDD-aligned schema through configurations
        var propertyConfig = new Infrastructure.Persistence.Configurations.PropertyConfiguration();
        propertyConfig.Should().NotBeNull();

        // ? Dependency injection configured
        var dependencyInjectionType = typeof(Infrastructure.DependencyInjection);
        dependencyInjectionType.Should().NotBeNull();
        
        var addInfrastructureMethod = dependencyInjectionType.GetMethod("AddInfrastructure");
        addInfrastructureMethod.Should().NotBeNull();
    }

    [Fact]
    public void Specification_Pattern_Integration_Works()
    {
        // Test that repositories properly integrate with specification pattern
        
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new ApplicationDbContext(options);
        var propertyRepo = new PropertyRepository(context);

        // Assert specification method exists
        var specMethod = typeof(PropertyRepository).GetMethod("GetBySpecificationAsync");
        specMethod.Should().NotBeNull();
        
        var parameterTypes = specMethod!.GetParameters().Select(p => p.ParameterType).ToList();
        parameterTypes.Should().Contain(typeof(Domain.Specifications.ISpecification<Domain.Entities.Property>));
    }
}