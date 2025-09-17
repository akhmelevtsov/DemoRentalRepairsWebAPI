using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using RentalRepairs.Application.Common.Behaviors;
using RentalRepairs.Application.Mappings;
using System.Reflection;

namespace RentalRepairs.Application;

public static class ConfigureServices
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register Mapster
        DomainToResponseMappingConfig.RegisterMappings();
        
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));
        });

        return services;
    }
}