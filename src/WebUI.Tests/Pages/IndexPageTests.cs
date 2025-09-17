using FluentAssertions;
using RentalRepairs.WebUI.Tests.Integration;
using System.Net;
using Xunit;

namespace RentalRepairs.WebUI.Tests.Pages;

public class IndexPageTests : IClassFixture<Step17InMemoryWebApplicationFactory<Program>>
{
    private readonly Step17InMemoryWebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public IndexPageTests(Step17InMemoryWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task Index_Page_Should_Return_Success_Or_Redirect()
    {
        // Act
        var response = await _client.GetAsync("/");

        // Assert - Should either load successfully or redirect to login
        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.OK, 
            HttpStatusCode.Redirect, 
            HttpStatusCode.Found,
            HttpStatusCode.PermanentRedirect);
    }

    [Fact]
    public async Task Index_Page_Should_Have_HTML_Content()
    {
        // Arrange
        var client = _factory.CreateClient(new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        // Act
        var response = await client.GetAsync("/");

        // Assert
        if (response.StatusCode == HttpStatusCode.OK)
        {
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("html", "Should return HTML content");
            content.Should().Contain("Dashboard", "Should contain dashboard title");
        }
        else
        {
            // If redirected, that's also acceptable for authenticated pages
            response.StatusCode.Should().BeOneOf(
                HttpStatusCode.Redirect, 
                HttpStatusCode.Found,
                HttpStatusCode.PermanentRedirect);
        }
    }

    [Fact]
    public async Task Index_Page_Should_Handle_Unauthenticated_Users()
    {
        // Arrange
        var client = _factory.CreateClient(new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        // Act
        var response = await client.GetAsync("/");

        // Assert
        // The page should either show content for unauthenticated users or redirect to login
        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.OK, 
            HttpStatusCode.Redirect, 
            HttpStatusCode.Found,
            HttpStatusCode.PermanentRedirect);
        
        if (response.StatusCode == HttpStatusCode.Redirect || 
            response.StatusCode == HttpStatusCode.Found ||
            response.StatusCode == HttpStatusCode.PermanentRedirect)
        {
            var location = response.Headers.Location?.ToString();
            location.Should().NotBeNull("Redirect should have a location header");
        }
    }
}