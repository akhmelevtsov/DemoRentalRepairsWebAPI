using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RentalRepairs.Infrastructure.Persistence;

namespace RentalRepairs.WebUI.Tests.Integration;

/// <summary>
/// Step 17: Custom WebApplicationFactory for End-to-End Integration Tests with In-Memory Database
/// This factory configures the application for integration testing with isolated in-memory database
/// </summary>
public class Step17InMemoryWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the existing ApplicationDbContext registration
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
            
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Add in-memory database for integration testing
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseInMemoryDatabase($"InMemoryDbForStep17Testing_{Guid.NewGuid()}");
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
            });

            // Configure logging for tests - minimize output
            services.AddLogging(builder =>
            {
                builder.ClearProviders();
                builder.SetMinimumLevel(LogLevel.Warning); // Reduce logging noise
            });

            // Build the service provider
            var serviceProvider = services.BuildServiceProvider();

            // Create a scope to obtain a reference to the database context
            using var scope = serviceProvider.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var context = scopedServices.GetRequiredService<ApplicationDbContext>();

            try
            {
                // Ensure the database is created
                context.Database.EnsureCreated();

                // Seed minimal test data
                SeedTestData(context);
            }
            catch (Exception)
            {
                // Ignore seeding errors in tests - they'll be handled per test
            }
        });

        builder.UseEnvironment("Testing");
    }

    private static void SeedTestData(ApplicationDbContext context)
    {
        try
        {
            // Minimal test data seeding
            context.SaveChanges();
        }
        catch (Exception)
        {
            // Ignore seeding errors
        }
    }
}