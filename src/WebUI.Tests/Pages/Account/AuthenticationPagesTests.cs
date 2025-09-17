using FluentAssertions;
using RentalRepairs.WebUI.Tests.Integration;
using System.Net;
using Xunit;

namespace RentalRepairs.WebUI.Tests.Pages.Account;

public class AuthenticationPagesTests : IClassFixture<Step17InMemoryWebApplicationFactory<Program>>
{
    private readonly Step17InMemoryWebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public AuthenticationPagesTests(Step17InMemoryWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task Login_Page_Should_Return_Success()
    {
        // Act
        var response = await _client.GetAsync("/Account/Login");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Login");
    }

    [Fact]
    public async Task Login_Page_Should_Have_Login_Form()
    {
        // Act
        var response = await _client.GetAsync("/Account/Login");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        content.Should().Contain("form");
        content.Should().Contain("Login");
    }

    [Theory]
    [InlineData("/Account/Login")]
    public async Task Authentication_Pages_Should_Be_Accessible(string url)
    {
        // Act
        var response = await _client.GetAsync(url);

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.Redirect);
    }

    [Fact]
    public async Task Login_Page_Should_Have_Basic_Structure()
    {
        // Act
        var response = await _client.GetAsync("/Account/Login");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        content.Should().Contain("html");
        content.Should().NotBeEmpty();
    }
}