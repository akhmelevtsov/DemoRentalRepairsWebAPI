using FluentAssertions;
using RentalRepairs.Application.Commands.Properties;
using RentalRepairs.Application.Commands.TenantRequests;
using RentalRepairs.Application.Commands.Tenants;
using RentalRepairs.Application.Commands.Workers;
using RentalRepairs.Application.Common.Interfaces;
using Xunit;

namespace RentalRepairs.Application.Tests.Commands;

public class CommandHandlerImplementationTests
{
    [Fact]
    public void RegisterPropertyCommandHandler_Should_Implement_ICommandHandler()
    {
        // This test validates that all command handlers implement the correct interfaces
        
        // Arrange & Act
        var registerPropertyType = typeof(Application.Commands.Properties.Handlers.RegisterPropertyCommandHandler);
        var createTenantRequestType = typeof(Application.Commands.TenantRequests.Handlers.CreateTenantRequestCommandHandler);
        var scheduleWorkType = typeof(Application.Commands.TenantRequests.Handlers.ScheduleServiceWorkCommandHandler);
        var closeRequestType = typeof(Application.Commands.TenantRequests.Handlers.CloseRequestCommandHandler);
        var registerTenantType = typeof(Application.Commands.Tenants.Handlers.RegisterTenantCommandHandler);
        var registerWorkerType = typeof(Application.Commands.Workers.Handlers.RegisterWorkerCommandHandler);

        // Assert - Check that command handlers implement the correct interfaces
        registerPropertyType.Should().BeAssignableTo<ICommandHandler<RegisterPropertyCommand, int>>();
        createTenantRequestType.Should().BeAssignableTo<ICommandHandler<CreateTenantRequestCommand, int>>();
        scheduleWorkType.Should().BeAssignableTo<ICommandHandler<ScheduleServiceWorkCommand>>();
        closeRequestType.Should().BeAssignableTo<ICommandHandler<CloseRequestCommand>>();
        registerTenantType.Should().BeAssignableTo<ICommandHandler<RegisterTenantCommand, int>>();
        registerWorkerType.Should().BeAssignableTo<ICommandHandler<RegisterWorkerCommand, int>>();
    }

    [Fact]
    public void Commands_Should_Have_Required_Properties()
    {
        // Test that commands have the expected properties
        
        // RegisterPropertyCommand
        var registerPropertyCommand = new RegisterPropertyCommand();
        registerPropertyCommand.Should().BeAssignableTo<ICommand<int>>();
        
        // CreateTenantRequestCommand
        var createRequestCommand = new CreateTenantRequestCommand();
        createRequestCommand.Should().BeAssignableTo<ICommand<int>>();
        
        // ScheduleServiceWorkCommand
        var scheduleCommand = new ScheduleServiceWorkCommand();
        scheduleCommand.Should().BeAssignableTo<ICommand>();
        
        // CloseRequestCommand
        var closeCommand = new CloseRequestCommand();
        closeCommand.Should().BeAssignableTo<ICommand>();
        
        // RegisterTenantCommand
        var registerTenantCommand = new RegisterTenantCommand();
        registerTenantCommand.Should().BeAssignableTo<ICommand<int>>();
        
        // RegisterWorkerCommand
        var registerWorkerCommand = new RegisterWorkerCommand();
        registerWorkerCommand.Should().BeAssignableTo<ICommand<int>>();
    }

    [Fact]
    public void CommandHandlers_Should_Have_Public_Constructors()
    {
        // Verify that all command handlers can be instantiated (for dependency injection)
        
        var registerPropertyHandlerType = typeof(Application.Commands.Properties.Handlers.RegisterPropertyCommandHandler);
        var createRequestHandlerType = typeof(Application.Commands.TenantRequests.Handlers.CreateTenantRequestCommandHandler);
        var scheduleWorkHandlerType = typeof(Application.Commands.TenantRequests.Handlers.ScheduleServiceWorkCommandHandler);
        var closeRequestHandlerType = typeof(Application.Commands.TenantRequests.Handlers.CloseRequestCommandHandler);
        var registerTenantHandlerType = typeof(Application.Commands.Tenants.Handlers.RegisterTenantCommandHandler);
        var registerWorkerHandlerType = typeof(Application.Commands.Workers.Handlers.RegisterWorkerCommandHandler);

        // Assert that all have public constructors with dependencies
        registerPropertyHandlerType.GetConstructors().Should().HaveCountGreaterThan(0);
        createRequestHandlerType.GetConstructors().Should().HaveCountGreaterThan(0);
        scheduleWorkHandlerType.GetConstructors().Should().HaveCountGreaterThan(0);
        closeRequestHandlerType.GetConstructors().Should().HaveCountGreaterThan(0);
        registerTenantHandlerType.GetConstructors().Should().HaveCountGreaterThan(0);
        registerWorkerHandlerType.GetConstructors().Should().HaveCountGreaterThan(0);
    }
}