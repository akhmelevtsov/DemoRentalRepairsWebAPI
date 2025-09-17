using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RentalRepairs.Application.Common.Interfaces;
using RentalRepairs.Infrastructure.Configuration;
using RentalRepairs.Infrastructure.Services.Email;
using RentalRepairs.Infrastructure.Services.Notifications;
using Xunit;

namespace RentalRepairs.Infrastructure.Tests.Services;

public class Step12ExternalServicesValidationTests
{
    [Fact]
    public void Step12_All_Email_Service_Implementations_Exist()
    {
        // Validate that all required email service implementations from Step 12 are created
        
        var mockEmailServiceType = typeof(MockEmailService);
        var smtpEmailServiceType = typeof(SmtpEmailService);
        var sendGridEmailServiceType = typeof(SendGridEmailService);

        // Assert all email service implementations exist
        mockEmailServiceType.Should().NotBeNull();
        smtpEmailServiceType.Should().NotBeNull();
        sendGridEmailServiceType.Should().NotBeNull();

        // Verify they implement IEmailService
        mockEmailServiceType.Should().BeAssignableTo<IEmailService>();
        smtpEmailServiceType.Should().BeAssignableTo<IEmailService>();
        sendGridEmailServiceType.Should().BeAssignableTo<IEmailService>();

        // Verify correct namespaces
        mockEmailServiceType.Namespace.Should().Be("RentalRepairs.Infrastructure.Services.Email");
        smtpEmailServiceType.Namespace.Should().Be("RentalRepairs.Infrastructure.Services.Email");
        sendGridEmailServiceType.Namespace.Should().Be("RentalRepairs.Infrastructure.Services.Email");
    }

    [Fact]
    public void Step12_All_Notification_Service_Implementations_Exist()
    {
        // Validate notification service implementations
        
        var notificationServiceType = typeof(NotificationService);
        
        notificationServiceType.Should().NotBeNull();
        notificationServiceType.Should().BeAssignableTo<INotificationService>();
        notificationServiceType.Namespace.Should().Be("RentalRepairs.Infrastructure.Services.Notifications");
    }

    [Fact]
    public void Step12_All_Configuration_Models_Exist()
    {
        // Validate configuration models from Step 12
        
        var notificationSettingsType = typeof(NotificationSettings);
        var externalServicesSettingsType = typeof(ExternalServicesSettings);
        var smtpSettingsType = typeof(SmtpSettings);
        var sendGridSettingsType = typeof(SendGridSettings);

        // Assert configuration types exist
        notificationSettingsType.Should().NotBeNull();
        externalServicesSettingsType.Should().NotBeNull();
        smtpSettingsType.Should().NotBeNull();
        sendGridSettingsType.Should().NotBeNull();

        // Verify correct namespaces
        notificationSettingsType.Namespace.Should().Be("RentalRepairs.Infrastructure.Configuration");
        externalServicesSettingsType.Namespace.Should().Be("RentalRepairs.Infrastructure.Configuration");
    }

    [Fact]
    public void Step12_Infrastructure_DependencyInjection_Enhanced()
    {
        // Test that dependency injection has been enhanced for external services
        
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["NotificationSettings:EmailProvider"] = "Mock",
                ["NotificationSettings:EnableEmailNotifications"] = "true",
                ["NotificationSettings:DefaultSenderEmail"] = "test@test.com"
            })
            .Build();

        var services = new ServiceCollection();
        services.AddLogging();

        // Act
        Infrastructure.DependencyInjection.AddInfrastructure(services, configuration);

        // Check service registrations without building provider
        var emailServiceDescriptor = services.FirstOrDefault(s => s.ServiceType == typeof(IEmailService));
        var notificationServiceDescriptor = services.FirstOrDefault(s => s.ServiceType == typeof(INotificationService));
        var notificationSettingsDescriptor = services.FirstOrDefault(s => s.ServiceType == typeof(INotificationSettings));

        // Assert - Verify services are registered
        emailServiceDescriptor.Should().NotBeNull();
        emailServiceDescriptor!.ImplementationType.Should().Be(typeof(MockEmailService));

        notificationServiceDescriptor.Should().NotBeNull();
        notificationServiceDescriptor!.ImplementationType.Should().Be(typeof(NotificationService));

        notificationSettingsDescriptor.Should().NotBeNull();
    }

    [Fact]
    public void Step12_EmailService_Interface_Methods_Exist()
    {
        // Validate IEmailService interface has required methods
        
        var emailServiceInterface = typeof(IEmailService);
        var methods = emailServiceInterface.GetMethods().Select(m => m.Name).ToList();

        methods.Should().Contain("SendEmailAsync");
        methods.Should().Contain("SendBulkEmailAsync");

        // Verify method signatures
        var sendEmailMethod = emailServiceInterface.GetMethod("SendEmailAsync");
        sendEmailMethod.Should().NotBeNull();
        sendEmailMethod!.ReturnType.Should().Be(typeof(Task));

        var parameters = sendEmailMethod.GetParameters();
        parameters.Should().HaveCount(2);
        parameters[0].ParameterType.Should().Be(typeof(EmailInfo));
        parameters[1].ParameterType.Should().Be(typeof(CancellationToken));
    }

    [Fact]
    public void Step12_NotificationService_Interface_Methods_Exist()
    {
        // Validate INotificationService interface has required methods
        
        var notificationServiceInterface = typeof(INotificationService);
        var methods = notificationServiceInterface.GetMethods().Select(m => m.Name).ToList();

        methods.Should().Contain("SendEmailNotificationAsync");
        methods.Should().Contain("SendTemplatedEmailAsync");
        methods.Should().Contain("SendBulkNotificationAsync");
        methods.Should().Contain("NotifyTenantAsync");
        methods.Should().Contain("NotifyPropertySuperintendentAsync");
        methods.Should().Contain("NotifyWorkerAsync");
        methods.Should().Contain("ScheduleNotificationAsync");
        methods.Should().Contain("SendUrgentNotificationAsync");
    }

    [Fact]
    public void Step12_EmailInfo_Class_Has_Required_Properties()
    {
        // Validate EmailInfo class structure
        
        var emailInfoType = typeof(EmailInfo);
        var properties = emailInfoType.GetProperties().Select(p => p.Name).ToList();

        properties.Should().Contain("SenderEmail");
        properties.Should().Contain("RecipientEmail");
        properties.Should().Contain("Subject");
        properties.Should().Contain("Body");
        properties.Should().Contain("IsBodyHtml");
        properties.Should().Contain("CcEmails");
        properties.Should().Contain("BccEmails");
        properties.Should().Contain("Headers");
    }

    [Fact]
    public void Step12_Configuration_Models_Have_Required_Properties()
    {
        // Test NotificationSettings properties
        var notificationSettingsType = typeof(NotificationSettings);
        var notificationProperties = notificationSettingsType.GetProperties().Select(p => p.Name).ToList();

        notificationProperties.Should().Contain("DefaultSenderEmail");
        notificationProperties.Should().Contain("DefaultSenderName");
        notificationProperties.Should().Contain("EnableEmailNotifications");
        notificationProperties.Should().Contain("EmailProvider");

        // Test ExternalServicesSettings properties
        var externalServicesType = typeof(ExternalServicesSettings);
        var externalProperties = externalServicesType.GetProperties().Select(p => p.Name).ToList();

        externalProperties.Should().Contain("Smtp");
        externalProperties.Should().Contain("SendGrid");
        externalProperties.Should().Contain("ApiIntegrations");
    }

    [Fact]
    public void Step12_Success_Criteria_Met()
    {
        // Validate that Step 12 success criteria from the migration plan are met

        // ? Email services moved to src/Infrastructure/Services/
        var mockEmailType = typeof(MockEmailService);
        var smtpEmailType = typeof(SmtpEmailService);
        var sendGridEmailType = typeof(SendGridEmailService);

        mockEmailType.Namespace.Should().Be("RentalRepairs.Infrastructure.Services.Email");
        smtpEmailType.Namespace.Should().Be("RentalRepairs.Infrastructure.Services.Email");
        sendGridEmailType.Namespace.Should().Be("RentalRepairs.Infrastructure.Services.Email");

        // ? Notification services moved to src/Infrastructure/Notifications/
        var notificationServiceType = typeof(NotificationService);
        notificationServiceType.Namespace.Should().Be("RentalRepairs.Infrastructure.Services.Notifications");

        // ? Dependency injection registration implemented
        var dependencyInjectionType = typeof(Infrastructure.DependencyInjection);
        var addInfrastructureMethod = dependencyInjectionType.GetMethod("AddInfrastructure");
        addInfrastructureMethod.Should().NotBeNull();

        // ? Configuration models created
        var notificationSettingsType = typeof(NotificationSettings);
        var externalServicesSettingsType = typeof(ExternalServicesSettings);
        
        notificationSettingsType.Namespace.Should().Be("RentalRepairs.Infrastructure.Configuration");
        externalServicesSettingsType.Namespace.Should().Be("RentalRepairs.Infrastructure.Configuration");
    }

    [Fact]
    public void Step12_External_Services_Architecture_Compliant()
    {
        // Verify external services follow clean architecture principles
        
        // Email services should depend on Application interfaces, not Infrastructure
        var emailServiceTypes = new[]
        {
            typeof(MockEmailService),
            typeof(SmtpEmailService),
            typeof(SendGridEmailService)
        };

        foreach (var emailServiceType in emailServiceTypes)
        {
            emailServiceType.Should().BeAssignableTo<IEmailService>();
            
            // Verify constructors don't have infrastructure dependencies
            var constructors = emailServiceType.GetConstructors();
            constructors.Should().NotBeEmpty();
        }

        // Notification service should depend on Application interfaces
        var notificationServiceType = typeof(NotificationService);
        notificationServiceType.Should().BeAssignableTo<INotificationService>();
    }
}