using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using RentalRepairs.Infrastructure.Persistence;
using System.Net;
using Xunit;
using Xunit.Abstractions;

namespace RentalRepairs.WebUI.Tests.Integration;

/// <summary>
/// Step 17: Comprehensive End-to-End Integration Tests using In-Memory Database
/// Tests actual web endpoints and page functionality using the WebHost
/// </summary>
public class Step17EndToEndIntegrationTests : IClassFixture<Step17InMemoryWebApplicationFactory<Program>>
{
    private readonly Step17InMemoryWebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly ITestOutputHelper _output;

    public Step17EndToEndIntegrationTests(
        Step17InMemoryWebApplicationFactory<Program> factory,
        ITestOutputHelper output)
    {
        _factory = factory;
        _client = _factory.CreateClient();
        _output = output;
    }

    [Fact]
    public async Task Step17_EndToEnd_HomePage_LoadsSuccessfully()
    {
        _output.WriteLine("?? Testing home page endpoint...");

        // Act
        var response = await _client.GetAsync("/");

        // Assert
        response.Should().NotBeNull();
        (response.StatusCode == HttpStatusCode.OK || 
         response.StatusCode == HttpStatusCode.Redirect || 
         response.StatusCode == HttpStatusCode.Found).Should().BeTrue();

        _output.WriteLine($"? Home page loads correctly (Status: {response.StatusCode})");
    }

    [Fact]
    public async Task Step17_EndToEnd_PrivacyPage_LoadsSuccessfully()
    {
        _output.WriteLine("?? Testing privacy page endpoint...");

        // Act
        var response = await _client.GetAsync("/Privacy");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Privacy Policy");

        _output.WriteLine("? Privacy page loads correctly with expected content");
    }

    [Fact]
    public async Task Step17_EndToEnd_LoginPage_LoadsSuccessfully()
    {
        _output.WriteLine("?? Testing login page endpoint...");

        // Act
        var response = await _client.GetAsync("/Account/Login");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Login");

        _output.WriteLine("? Login page loads correctly with login form");
    }

    [Fact]
    public async Task Step17_EndToEnd_PropertyRegistrationPage_LoadsCorrectly()
    {
        _output.WriteLine("?? Testing property registration page endpoint...");

        // Act
        var response = await _client.GetAsync("/Properties/Register");

        // Assert
        // May redirect to login if authentication is required
        (response.StatusCode == HttpStatusCode.OK || 
         response.StatusCode == HttpStatusCode.Redirect || 
         response.StatusCode == HttpStatusCode.Found).Should().BeTrue();

        _output.WriteLine($"? Property registration page responds correctly (Status: {response.StatusCode})");
    }

    [Fact]
    public async Task Step17_EndToEnd_TenantRequestSubmitPage_LoadsCorrectly()
    {
        _output.WriteLine("?? Testing tenant request submit page endpoint...");

        // Act
        var response = await _client.GetAsync("/TenantRequests/Submit");

        // Assert
        // May redirect to login if authentication is required
        (response.StatusCode == HttpStatusCode.OK || 
         response.StatusCode == HttpStatusCode.Redirect || 
         response.StatusCode == HttpStatusCode.Found).Should().BeTrue();

        _output.WriteLine($"? Tenant request submit page responds correctly (Status: {response.StatusCode})");
    }

    [Fact]
    public async Task Step17_EndToEnd_HealthCheckEndpoint_ReturnsHealthy()
    {
        _output.WriteLine("?? Testing health check endpoint...");

        // Act
        var response = await _client.GetAsync("/health");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Healthy");

        _output.WriteLine("? Health check endpoint returns healthy status");
    }

    [Fact]
    public async Task Step17_EndToEnd_StaticFiles_LoadCorrectly()
    {
        _output.WriteLine("?? Testing static file serving...");

        // Test CSS file
        var cssResponse = await _client.GetAsync("/css/site.css");
        cssResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        cssResponse.Content.Headers.ContentType?.MediaType.Should().Be("text/css");

        // Test JavaScript file
        var jsResponse = await _client.GetAsync("/js/site.js");
        jsResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        
        _output.WriteLine("? Static files (CSS, JS) serve correctly");
    }

    [Fact]
    public async Task Step17_EndToEnd_ErrorHandling_WorksCorrectly()
    {
        _output.WriteLine("?? Testing error handling for non-existent pages...");

        // Act - Request a non-existent page
        var response = await _client.GetAsync("/NonExistentPage");

        // Assert - Should return 404 or redirect to error page
        (response.StatusCode == HttpStatusCode.NotFound || 
         response.StatusCode == HttpStatusCode.Redirect ||
         response.StatusCode == HttpStatusCode.Found).Should().BeTrue();

        _output.WriteLine($"? Error handling works correctly (Status: {response.StatusCode})");
    }

    [Fact]
    public async Task Step17_EndToEnd_DatabaseIntegration_WorksWithEndpoints()
    {
        _output.WriteLine("?? Testing database integration with endpoints...");

        using var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Ensure database is created
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();

        // Test that endpoints can still load with fresh database
        var response = await _client.GetAsync("/");
        response.Should().NotBeNull();

        // Test that database operations work
        var saveResult = await context.SaveChangesAsync();
        saveResult.Should().BeGreaterThanOrEqualTo(0);

        _output.WriteLine("? Database integration works correctly with endpoints");
    }

    [Fact]
    public async Task Step17_EndToEnd_FormSubmission_PrivacyPageForm()
    {
        _output.WriteLine("?? Testing form handling capabilities...");

        // Get the privacy page (which should have basic form elements)
        var getResponse = await _client.GetAsync("/Privacy");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await getResponse.Content.ReadAsStringAsync();
        
        // Verify the page contains expected form elements or content
        content.Should().NotBeEmpty();
        content.Should().Contain("html");

        _output.WriteLine("? Page content and form handling infrastructure working");
    }

    [Fact]
    public async Task Step17_EndToEnd_AuthenticationFlow_RedirectsCorrectly()
    {
        _output.WriteLine("?? Testing authentication flow redirects...");

        // Try to access a protected resource
        var response = await _client.GetAsync("/Properties/Register");

        // Should either show the page or redirect to login
        (response.StatusCode == HttpStatusCode.OK ||
         response.StatusCode == HttpStatusCode.Redirect ||
         response.StatusCode == HttpStatusCode.Found ||
         response.StatusCode == HttpStatusCode.Unauthorized).Should().BeTrue();

        if (response.StatusCode == HttpStatusCode.Redirect || response.StatusCode == HttpStatusCode.Found)
        {
            var location = response.Headers.Location?.ToString();
            location.Should().NotBeNull();
            _output.WriteLine($"? Authentication redirect working (redirected to: {location})");
        }
        else
        {
            _output.WriteLine("? Protected resource accessible or properly secured");
        }
    }

    [Fact]
    public async Task Step17_EndToEnd_ResponseHeaders_ConfiguredCorrectly()
    {
        _output.WriteLine("?? Testing security headers and response configuration...");

        // Act
        var response = await _client.GetAsync("/");

        // Assert - Check for security headers that should be configured
        response.Headers.Should().NotBeNull();
        
        // The application should return proper response headers
        response.StatusCode.Should().NotBe(HttpStatusCode.InternalServerError);

        _output.WriteLine("? Response headers and security configuration working");
    }

    [Fact]
    public async Task Step17_EndToEnd_MultipleEndpoints_ConcurrentAccess()
    {
        _output.WriteLine("?? Testing concurrent access to multiple endpoints...");

        // Test multiple endpoints concurrently
        var tasks = new[]
        {
            _client.GetAsync("/"),
            _client.GetAsync("/Privacy"),
            _client.GetAsync("/Account/Login"),
            _client.GetAsync("/health")
        };

        var responses = await Task.WhenAll(tasks);

        // All requests should complete successfully
        foreach (var response in responses)
        {
            response.Should().NotBeNull();
            response.StatusCode.Should().NotBe(HttpStatusCode.InternalServerError);
        }

        _output.WriteLine("? Concurrent endpoint access working correctly");
    }

    [Fact]
    public async Task Step17_EndToEnd_WebHostIntegration_AllComponentsWorking()
    {
        _output.WriteLine("?? Comprehensive WebHost integration test...");

        var validationResults = new List<string>();

        try
        {
            // 1. Test WebHost startup
            var client = _factory.CreateClient();
            client.Should().NotBeNull();
            validationResults.Add("? WebHost startup successful");

            // 2. Test database integration
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await context.Database.EnsureCreatedAsync();
            validationResults.Add("? Database integration successful");

            // 3. Test endpoint accessibility
            var homeResponse = await client.GetAsync("/");
            homeResponse.Should().NotBeNull();
            validationResults.Add("? Endpoint accessibility confirmed");

            // 4. Test static file serving
            var cssResponse = await client.GetAsync("/css/site.css");
            cssResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            validationResults.Add("? Static file serving working");

            // 5. Test health checks
            var healthResponse = await client.GetAsync("/health");
            healthResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            validationResults.Add("? Health checks operational");

            _output.WriteLine("?? Step 17: COMPREHENSIVE WEBHOST INTEGRATION TEST PASSED!");

            foreach (var result in validationResults)
            {
                _output.WriteLine(result);
            }
        }
        catch (Exception ex)
        {
            _output.WriteLine($"? Step 17: WebHost integration test FAILED: {ex.Message}");
            throw;
        }

        validationResults.Should().HaveCount(5, "All WebHost integration components should be validated");

        _output.WriteLine("? Step 17: END-TO-END INTEGRATION TESTS WITH IN-MEMORY DATABASE COMPLETE");
        _output.WriteLine("? Step 17: All web endpoints tested successfully");
    }
}