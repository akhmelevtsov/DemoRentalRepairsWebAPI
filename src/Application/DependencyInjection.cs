using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using RentalRepairs.Application.Common.Behaviors;
using RentalRepairs.Application.Interfaces;
using RentalRepairs.Application.Services;
using RentalRepairs.Application.Mappings;
using RentalRepairs.Domain;

namespace RentalRepairs.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Register Domain Services first
        services.AddDomain();

        // Register MediatR
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });

        // Register Mapster
        DomainToResponseMappingConfig.RegisterMappings();

        // Register FluentValidation
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        // Register MediatR behaviors
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));

        // Register Application Services
        services.AddScoped<IPropertyService, PropertyService>();
        services.AddScoped<ITenantRequestService, TenantRequestService>();
        services.AddScoped<IWorkerService, WorkerService>();
        services.AddScoped<INotifyPartiesService, NotifyPartiesService>();

        return services;
    }
}