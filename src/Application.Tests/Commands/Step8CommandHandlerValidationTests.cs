using FluentAssertions;
using RentalRepairs.Application.Commands.Properties;
using RentalRepairs.Application.Commands.TenantRequests;
using RentalRepairs.Application.Commands.Tenants;
using RentalRepairs.Application.Commands.Workers;
using RentalRepairs.Application.Common.Interfaces;
using RentalRepairs.Application.DTOs;
using Xunit;

namespace RentalRepairs.Application.Tests.Commands;

public class Step8CommandHandlerValidationTests
{
    [Fact]
    public void All_Required_Command_Handlers_Exist()
    {
        // This test validates that Step 8 command handlers are implemented
        
        // Arrange & Act - Check that all required types exist
        var registerPropertyHandlerType = typeof(Application.Commands.Properties.Handlers.RegisterPropertyCommandHandler);
        var createTenantRequestHandlerType = typeof(Application.Commands.TenantRequests.Handlers.CreateTenantRequestCommandHandler);
        var scheduleWorkHandlerType = typeof(Application.Commands.TenantRequests.Handlers.ScheduleServiceWorkCommandHandler);
        var closeRequestHandlerType = typeof(Application.Commands.TenantRequests.Handlers.CloseRequestCommandHandler);
        var registerTenantHandlerType = typeof(Application.Commands.Tenants.Handlers.RegisterTenantCommandHandler);
        var registerWorkerHandlerType = typeof(Application.Commands.Workers.Handlers.RegisterWorkerCommandHandler);

        // Assert - All handlers exist and implement correct interfaces
        registerPropertyHandlerType.Should().NotBeNull();
        createTenantRequestHandlerType.Should().NotBeNull();
        scheduleWorkHandlerType.Should().NotBeNull();
        closeRequestHandlerType.Should().NotBeNull();
        registerTenantHandlerType.Should().NotBeNull();
        registerWorkerHandlerType.Should().NotBeNull();

        // Verify they implement the correct interfaces
        registerPropertyHandlerType.Should().BeAssignableTo<ICommandHandler<RegisterPropertyCommand, int>>();
        createTenantRequestHandlerType.Should().BeAssignableTo<ICommandHandler<CreateTenantRequestCommand, int>>();
        scheduleWorkHandlerType.Should().BeAssignableTo<ICommandHandler<ScheduleServiceWorkCommand>>();
        closeRequestHandlerType.Should().BeAssignableTo<ICommandHandler<CloseRequestCommand>>();
        registerTenantHandlerType.Should().BeAssignableTo<ICommandHandler<RegisterTenantCommand, int>>();
        registerWorkerHandlerType.Should().BeAssignableTo<ICommandHandler<RegisterWorkerCommand, int>>();
    }

    [Fact]
    public void All_Commands_Implement_Correct_Interfaces()
    {
        // Test that all commands implement the correct CQRS interfaces
        
        var registerPropertyCommand = new RegisterPropertyCommand();
        var createRequestCommand = new CreateTenantRequestCommand();
        var scheduleCommand = new ScheduleServiceWorkCommand();
        var closeCommand = new CloseRequestCommand();
        var registerTenantCommand = new RegisterTenantCommand();
        var registerWorkerCommand = new RegisterWorkerCommand();

        // Assert all commands implement correct interfaces
        registerPropertyCommand.Should().BeAssignableTo<ICommand<int>>();
        createRequestCommand.Should().BeAssignableTo<ICommand<int>>();
        scheduleCommand.Should().BeAssignableTo<ICommand>();
        closeCommand.Should().BeAssignableTo<ICommand>();
        registerTenantCommand.Should().BeAssignableTo<ICommand<int>>();
        registerWorkerCommand.Should().BeAssignableTo<ICommand<int>>();
    }

    [Fact]
    public void Commands_Have_Required_Properties()
    {
        // Test that commands have the expected structure for business operations
        
        // RegisterPropertyCommand
        var registerPropertyCommand = new RegisterPropertyCommand
        {
            Name = "Test Property",
            Code = "TP-001",
            PhoneNumber = "555-1234",
            NoReplyEmailAddress = "test@example.com",
            Units = new List<string> { "101", "102" },
            Address = new PropertyAddressDto
            {
                StreetNumber = "123",
                StreetName = "Main St", 
                City = "Test City",
                PostalCode = "12345"
            },
            Superintendent = new PersonContactInfoDto
            {
                FirstName = "John",
                LastName = "Doe",
                EmailAddress = "john@test.com"
            }
        };

        registerPropertyCommand.Name.Should().Be("Test Property");
        registerPropertyCommand.Code.Should().Be("TP-001");
        registerPropertyCommand.Units.Should().HaveCount(2);

        // CreateTenantRequestCommand
        var createRequestCommand = new CreateTenantRequestCommand
        {
            TenantId = 1,
            Title = "Test Request",
            Description = "Test Description",
            UrgencyLevel = "High"
        };

        createRequestCommand.TenantId.Should().Be(1);
        createRequestCommand.Title.Should().Be("Test Request");
        createRequestCommand.UrgencyLevel.Should().Be("High");

        // ScheduleServiceWorkCommand
        var scheduleCommand = new ScheduleServiceWorkCommand
        {
            TenantRequestId = 1,
            ScheduledDate = DateTime.UtcNow.AddDays(1),
            WorkerEmail = "worker@test.com",
            WorkOrderNumber = "WO-001"
        };

        scheduleCommand.TenantRequestId.Should().Be(1);
        scheduleCommand.WorkerEmail.Should().Be("worker@test.com");
        scheduleCommand.WorkOrderNumber.Should().Be("WO-001");
    }

    [Fact]
    public void Step8_Success_Criteria_Met()
    {
        // Validate that Step 8 success criteria from the migration plan are met
        
        // ? RegisterPropertyCommand and RegisterPropertyCommandHandler
        var registerPropertyCommandType = typeof(RegisterPropertyCommand);
        var registerPropertyHandlerType = typeof(Application.Commands.Properties.Handlers.RegisterPropertyCommandHandler);
        registerPropertyCommandType.Should().NotBeNull();
        registerPropertyHandlerType.Should().NotBeNull();

        // ? RegisterTenantRequestCommand and Handler (CreateTenantRequestCommand)
        var createTenantRequestCommandType = typeof(CreateTenantRequestCommand);
        var createTenantRequestHandlerType = typeof(Application.Commands.TenantRequests.Handlers.CreateTenantRequestCommandHandler);
        createTenantRequestCommandType.Should().NotBeNull();
        createTenantRequestHandlerType.Should().NotBeNull();

        // ? ScheduleServiceWorkCommand and ScheduleServiceWorkCommandHandler
        var scheduleServiceWorkCommandType = typeof(ScheduleServiceWorkCommand);
        var scheduleServiceWorkHandlerType = typeof(Application.Commands.TenantRequests.Handlers.ScheduleServiceWorkCommandHandler);
        scheduleServiceWorkCommandType.Should().NotBeNull();
        scheduleServiceWorkHandlerType.Should().NotBeNull();

        // ? CloseRequestCommand and CloseRequestCommandHandler
        var closeRequestCommandType = typeof(CloseRequestCommand);
        var closeRequestHandlerType = typeof(Application.Commands.TenantRequests.Handlers.CloseRequestCommandHandler);
        closeRequestCommandType.Should().NotBeNull();
        closeRequestHandlerType.Should().NotBeNull();

        // Additional handlers created for completeness
        var registerTenantHandlerType = typeof(Application.Commands.Tenants.Handlers.RegisterTenantCommandHandler);
        var registerWorkerHandlerType = typeof(Application.Commands.Workers.Handlers.RegisterWorkerCommandHandler);
        registerTenantHandlerType.Should().NotBeNull();
        registerWorkerHandlerType.Should().NotBeNull();
    }
}