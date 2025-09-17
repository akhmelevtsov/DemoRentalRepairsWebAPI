using FluentAssertions;
using RentalRepairs.WebUI.Tests.Integration;
using System.Net;
using Xunit;

namespace RentalRepairs.WebUI.Tests.Pages;

public class BasicPageTests : IClassFixture<Step17InMemoryWebApplicationFactory<Program>>
{
    private readonly Step17InMemoryWebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public BasicPageTests(Step17InMemoryWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task Index_Page_Should_Be_Accessible()
    {
        // Act
        var response = await _client.GetAsync("/");

        // Assert
        // Should either return OK or redirect to login (both are acceptable)
        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.OK, 
            HttpStatusCode.Redirect, 
            HttpStatusCode.Found,
            HttpStatusCode.PermanentRedirect);
    }

    [Fact]
    public async Task Login_Page_Should_Be_Accessible()
    {
        // Act
        var response = await _client.GetAsync("/Account/Login");

        // Assert
        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.OK, 
            HttpStatusCode.Redirect, 
            HttpStatusCode.Found,
            HttpStatusCode.PermanentRedirect);
    }

    [Fact]
    public async Task Property_Register_Page_Should_Be_Accessible()
    {
        // Act
        var response = await _client.GetAsync("/Properties/Register");

        // Assert
        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.OK, 
            HttpStatusCode.Redirect, 
            HttpStatusCode.Found,
            HttpStatusCode.PermanentRedirect);
    }

    [Fact]
    public async Task Tenant_Request_Submit_Page_Should_Be_Accessible()
    {
        // Act
        var response = await _client.GetAsync("/TenantRequests/Submit");

        // Assert
        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.OK, 
            HttpStatusCode.Redirect, 
            HttpStatusCode.Found,
            HttpStatusCode.PermanentRedirect);
    }
}