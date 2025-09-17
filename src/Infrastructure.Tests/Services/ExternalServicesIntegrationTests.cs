using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RentalRepairs.Application.Common.Interfaces;
using RentalRepairs.Infrastructure.Configuration;
using RentalRepairs.Infrastructure.Services.Email;
using RentalRepairs.Infrastructure.Services.Notifications;
using Xunit;

namespace RentalRepairs.Infrastructure.Tests.Services;

public class ExternalServicesIntegrationTests
{
    [Fact]
    public void MockEmailService_Should_Send_And_Track_Emails()
    {
        // Arrange
        var logger = new MockLogger<MockEmailService>();
        var mockEmailService = new MockEmailService(logger);

        var emailInfo = new EmailInfo
        {
            SenderEmail = "sender@test.com",
            RecipientEmail = "recipient@test.com",
            Subject = "Test Email",
            Body = "This is a test email",
            IsBodyHtml = true
        };

        // Act
        var sendTask = mockEmailService.SendEmailAsync(emailInfo);

        // Assert
        sendTask.Should().NotBeNull();
        sendTask.IsCompleted.Should().BeTrue();
        
        mockEmailService.SentEmails.Should().HaveCount(1);
        mockEmailService.LastSentEmail.Should().NotBeNull();
        mockEmailService.LastSentEmail!.Subject.Should().Be("Test Email");
        mockEmailService.LastSentEmail.RecipientEmail.Should().Be("recipient@test.com");
    }

    [Fact]
    public void MockEmailService_Should_Handle_Multiple_Emails()
    {
        // Arrange
        var logger = new MockLogger<MockEmailService>();
        var mockEmailService = new MockEmailService(logger);

        var emails = new[]
        {
            new EmailInfo { RecipientEmail = "user1@test.com", Subject = "Email 1", Body = "Body 1" },
            new EmailInfo { RecipientEmail = "user2@test.com", Subject = "Email 2", Body = "Body 2" },
            new EmailInfo { RecipientEmail = "user3@test.com", Subject = "Email 3", Body = "Body 3" }
        };

        // Act
        var sendTask = mockEmailService.SendBulkEmailAsync(emails);

        // Assert
        sendTask.Should().NotBeNull();
        sendTask.IsCompleted.Should().BeTrue();
        
        mockEmailService.SentEmails.Should().HaveCount(3);
        mockEmailService.SentEmails.Should().OnlyContain(email => 
            email.RecipientEmail.Contains("@test.com"));
    }

    [Fact]
    public void NotificationSettings_Should_Be_Configured_Correctly()
    {
        // Arrange
        var settings = new NotificationSettings
        {
            DefaultSenderEmail = "test@example.com",
            DefaultSenderName = "Test System",
            EnableEmailNotifications = true,
            EmailProvider = EmailProvider.Mock
        };

        var adapter = new NotificationSettingsAdapter(settings);

        // Assert
        adapter.DefaultSenderEmail.Should().Be("test@example.com");
        adapter.DefaultSenderName.Should().Be("Test System");
        adapter.EnableEmailNotifications.Should().BeTrue();
    }

    [Fact]
    public void EmailInfo_Should_Support_All_Email_Features()
    {
        // Arrange & Act
        var emailInfo = new EmailInfo
        {
            SenderEmail = "sender@test.com",
            RecipientEmail = "recipient@test.com",
            Subject = "Test Subject",
            Body = "<h1>Test Body</h1>",
            IsBodyHtml = true
        };

        emailInfo.CcEmails.Add("cc@test.com");
        emailInfo.BccEmails.Add("bcc@test.com");
        emailInfo.Headers.Add("X-Custom-Header", "CustomValue");

        // Assert
        emailInfo.SenderEmail.Should().Be("sender@test.com");
        emailInfo.RecipientEmail.Should().Be("recipient@test.com");
        emailInfo.Subject.Should().Be("Test Subject");
        emailInfo.Body.Should().Be("<h1>Test Body</h1>");
        emailInfo.IsBodyHtml.Should().BeTrue();
        emailInfo.CcEmails.Should().Contain("cc@test.com");
        emailInfo.BccEmails.Should().Contain("bcc@test.com");
        emailInfo.Headers.Should().ContainKey("X-Custom-Header");
        emailInfo.Headers["X-Custom-Header"].Should().Be("CustomValue");
    }

    [Fact]
    public void SmtpEmailOptions_Should_Have_Sensible_Defaults()
    {
        // Arrange & Act
        var options = new SmtpEmailOptions();

        // Assert
        options.Host.Should().Be("localhost");
        options.Port.Should().Be(587);
        options.EnableSsl.Should().BeTrue();
        options.EnableAuthentication.Should().BeTrue();
        options.DefaultSenderName.Should().Be("Rental Repairs System");
    }

    [Fact]
    public void ExternalServicesSettings_Should_Be_Configurable()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ExternalServices:Smtp:Host"] = "smtp.test.com",
                ["ExternalServices:Smtp:Port"] = "25",
                ["ExternalServices:SendGrid:ApiKey"] = "test-key",
                ["NotificationSettings:DefaultSenderEmail"] = "noreply@test.com",
                ["NotificationSettings:EmailProvider"] = "Smtp"
            })
            .Build();

        // Act
        var externalServicesSettings = new ExternalServicesSettings();
        configuration.GetSection(ExternalServicesSettings.SectionName).Bind(externalServicesSettings);

        var notificationSettings = new NotificationSettings();
        configuration.GetSection(NotificationSettings.SectionName).Bind(notificationSettings);

        // Assert
        externalServicesSettings.Smtp.Host.Should().Be("smtp.test.com");
        externalServicesSettings.Smtp.Port.Should().Be(25);
        externalServicesSettings.SendGrid.ApiKey.Should().Be("test-key");
        notificationSettings.DefaultSenderEmail.Should().Be("noreply@test.com");
        notificationSettings.EmailProvider.Should().Be(EmailProvider.Smtp);
    }

    // Mock logger for testing
    public class MockLogger<T> : ILogger<T>
    {
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
        public bool IsEnabled(LogLevel logLevel) => true;
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) { }
    }
}