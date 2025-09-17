using Microsoft.Extensions.DependencyInjection;
using RentalRepairs.Domain.Services;

namespace RentalRepairs.Domain;

public static class DependencyInjection
{
    public static IServiceCollection AddDomain(this IServiceCollection services)
    {
        // Register Domain Services
        services.AddScoped<PropertyDomainService>();
        services.AddScoped<TenantRequestDomainService>();
        services.AddScoped<WorkerAssignmentService>();
        services.AddScoped<RequestPrioritizationService>();
        services.AddScoped<DomainValidationService>();
        services.AddScoped<BusinessRulesEngine>();

        return services;
    }
}