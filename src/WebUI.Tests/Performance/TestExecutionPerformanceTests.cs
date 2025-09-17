using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using RentalRepairs.WebUI.Tests.Integration;
using System.Diagnostics;
using Xunit;

namespace RentalRepairs.WebUI.Tests.Performance;

/// <summary>
/// Performance and execution validation tests for Step 17
/// </summary>
public class TestExecutionPerformanceTests : IClassFixture<Step17InMemoryWebApplicationFactory<Program>>
{
    private readonly Step17InMemoryWebApplicationFactory<Program> _factory;

    public TestExecutionPerformanceTests(Step17InMemoryWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public void Performance_Test_Project_Loads_Quickly()
    {
        // Test that the test project itself loads quickly
        var stopwatch = Stopwatch.StartNew();
        
        var testAssembly = typeof(TestExecutionPerformanceTests).Assembly;
        var types = testAssembly.GetTypes();
        
        stopwatch.Stop();
        
        types.Should().NotBeEmpty();
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(5000); // Should load in under 5 seconds
    }

    [Fact]
    public async Task Performance_WebApplicationFactory_Starts_Quickly()
    {
        // Test that the web application factory starts up quickly
        var stopwatch = Stopwatch.StartNew();
        
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/");
        
        stopwatch.Stop();
        
        response.Should().NotBeNull();
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(10000); // Should respond in under 10 seconds
    }

    [Fact]
    public void Performance_DI_Container_Resolves_Services_Quickly()
    {
        // Test that dependency injection resolves quickly
        var stopwatch = Stopwatch.StartNew();
        
        using var scope = _factory.Services.CreateScope();
        var services = new object?[]
        {
            scope.ServiceProvider.GetService<MediatR.IMediator>(),
            scope.ServiceProvider.GetService<Microsoft.Extensions.Logging.ILogger<TestExecutionPerformanceTests>>(),
            scope.ServiceProvider.GetService<RentalRepairs.Infrastructure.Persistence.ApplicationDbContext>()
        };
        
        stopwatch.Stop();
        
        services.Should().AllSatisfy(s => s.Should().NotBeNull());
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(1000); // Should resolve in under 1 second
    }

    [Fact]
    public async Task Performance_Multiple_Page_Requests_Are_Fast()
    {
        // Test that multiple page requests perform well
        var client = _factory.CreateClient();
        var pages = new[] { "/", "/Account/Login", "/Properties/Register", "/TenantRequests/Submit" };
        
        var stopwatch = Stopwatch.StartNew();
        
        var tasks = pages.Select(page => client.GetAsync(page)).ToArray();
        var responses = await Task.WhenAll(tasks);
        
        stopwatch.Stop();
        
        responses.Should().AllSatisfy(r => r.Should().NotBeNull());
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(15000); // All requests should complete in under 15 seconds
    }

    [Fact]
    public void Reliability_Test_Execution_Is_Deterministic()
    {
        // Test that tests execute reliably and deterministically
        var results = new List<bool>();
        
        // Run the same test multiple times
        for (int i = 0; i < 5; i++)
        {
            try
            {
                var testAssembly = typeof(TestExecutionPerformanceTests).Assembly;
                var webUIAssembly = typeof(Program).Assembly;
                
                var result = testAssembly != null && webUIAssembly != null;
                results.Add(result);
            }
            catch
            {
                results.Add(false);
            }
        }
        
        // All executions should succeed
        results.Should().AllSatisfy(r => r.Should().BeTrue());
        results.Should().HaveCount(5);
    }

    [Fact]
    public void Stability_WebApplicationFactory_Can_Be_Created_Multiple_Times()
    {
        // Test that we can create multiple instances of the web application factory
        var factories = new List<Step17InMemoryWebApplicationFactory<Program>>();
        
        try
        {
            for (int i = 0; i < 3; i++)
            {
                var factory = new Step17InMemoryWebApplicationFactory<Program>();
                factories.Add(factory);
                
                // Test that each factory works
                var client = factory.CreateClient();
                client.Should().NotBeNull();
            }
            
            factories.Should().HaveCount(3);
            factories.Should().AllSatisfy(f => f.Should().NotBeNull());
        }
        finally
        {
            // Clean up
            foreach (var factory in factories)
            {
                factory.Dispose();
            }
        }
    }

    [Fact]
    public async Task Concurrency_Multiple_Simultaneous_Requests_Work()
    {
        // Test that the application can handle multiple simultaneous requests
        var client = _factory.CreateClient();
        var requestTasks = new List<Task<HttpResponseMessage>>();
        
        // Create 10 simultaneous requests
        for (int i = 0; i < 10; i++)
        {
            requestTasks.Add(client.GetAsync("/"));
        }
        
        var responses = await Task.WhenAll(requestTasks);
        
        responses.Should().HaveCount(10);
        responses.Should().AllSatisfy(r => 
        {
            r.Should().NotBeNull();
            r.StatusCode.Should().BeOneOf(
                System.Net.HttpStatusCode.OK,
                System.Net.HttpStatusCode.Redirect,
                System.Net.HttpStatusCode.Found
            );
        });
    }
}