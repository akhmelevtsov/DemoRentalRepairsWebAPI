using FluentAssertions;
using RentalRepairs.WebUI.Tests.Integration;
using System.Net;
using Xunit;

namespace RentalRepairs.WebUI.Tests.Pages.TenantRequests;

public class TenantRequestPagesTests : IClassFixture<Step17InMemoryWebApplicationFactory<Program>>
{
    private readonly Step17InMemoryWebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public TenantRequestPagesTests(Step17InMemoryWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task Submit_Request_Page_Should_Be_Accessible()
    {
        // Arrange
        var client = _factory.CreateClient(new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });

        // Act
        var response = await client.GetAsync("/TenantRequests/Submit");

        // Assert - Page should either load or redirect (if authentication required)
        response.StatusCode.Should().BeOneOf(
            HttpStatusCode.OK, 
            HttpStatusCode.Redirect, 
            HttpStatusCode.Found, 
            HttpStatusCode.Unauthorized,
            HttpStatusCode.PermanentRedirect);
    }

    [Fact]
    public async Task Submit_Request_Page_Should_Have_Form_When_Accessible()
    {
        // Act
        var response = await _client.GetAsync("/TenantRequests/Submit");

        // Assert
        if (response.StatusCode == HttpStatusCode.OK)
        {
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("form", "Should contain a form element");
            content.Should().Contain("Request", "Should contain request-related content");
        }
        else
        {
            // If not OK, should be a redirect (which is acceptable for protected pages)
            response.StatusCode.Should().BeOneOf(
                HttpStatusCode.Redirect, 
                HttpStatusCode.Found, 
                HttpStatusCode.Unauthorized,
                HttpStatusCode.PermanentRedirect);
        }
    }

    [Theory]
    [InlineData("/TenantRequests/Submit")]
    public async Task TenantRequest_Pages_Should_Respond(string url)
    {
        // Act
        var response = await _client.GetAsync(url);

        // Assert - Should get some kind of response (not internal server error)
        response.StatusCode.Should().NotBe(HttpStatusCode.InternalServerError);
        response.Should().NotBeNull();
    }
}