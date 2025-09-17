using Microsoft.Extensions.DependencyInjection;
using RentalRepairs.Domain.Services;

namespace RentalRepairs.Domain;

public static class DomainServiceCollectionExtensions
{
    public static IServiceCollection AddDomainServices(this IServiceCollection services)
    {
        // Register domain services
        services.AddScoped<PropertyDomainService>();
        services.AddScoped<TenantRequestDomainService>();
        services.AddScoped<WorkerAssignmentService>();
        services.AddScoped<RequestPrioritizationService>();
        services.AddScoped<DomainValidationService>();
        services.AddScoped<BusinessRulesEngine>();

        return services;
    }
}