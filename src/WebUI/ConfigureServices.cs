using RentalRepairs.Application;
using RentalRepairs.Application.Common.Interfaces;
using RentalRepairs.Infrastructure;
using RentalRepairs.WebUI.Services;

namespace RentalRepairs.WebUI;

public static class ConfigureServices
{
    public static IServiceCollection AddWebUIServices(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        services.AddRazorPages();

        return services;
    }
}