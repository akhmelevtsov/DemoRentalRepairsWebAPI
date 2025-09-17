using FluentAssertions;
using RentalRepairs.WebUI.Models;
using Xunit;

namespace RentalRepairs.WebUI.Tests.Models;

public class ViewModelValidationTests
{
    [Fact]
    public void Step14_All_Property_ViewModels_Exist()
    {
        // Validate that all required view models from Step 14 are created
        var registerPropertyViewModel = typeof(RegisterPropertyViewModel);
        var propertyDetailsViewModel = typeof(PropertyDetailsViewModel);
        var propertyListViewModel = typeof(PropertyListViewModel);
        var propertySummaryViewModel = typeof(PropertySummaryViewModel);

        // Assert all view model types exist
        registerPropertyViewModel.Should().NotBeNull();
        propertyDetailsViewModel.Should().NotBeNull();
        propertyListViewModel.Should().NotBeNull();
        propertySummaryViewModel.Should().NotBeNull();

        // Verify correct namespaces
        registerPropertyViewModel.Namespace.Should().Be("RentalRepairs.WebUI.Models");
        propertyDetailsViewModel.Namespace.Should().Be("RentalRepairs.WebUI.Models");
        propertyListViewModel.Namespace.Should().Be("RentalRepairs.WebUI.Models");
        propertySummaryViewModel.Namespace.Should().Be("RentalRepairs.WebUI.Models");
    }

    [Fact]
    public void Step14_All_TenantRequest_ViewModels_Exist()
    {
        // Validate that all required view models from Step 14 are created
        var submitTenantRequestViewModel = typeof(SubmitTenantRequestViewModel);
        var tenantRequestDetailsViewModel = typeof(TenantRequestDetailsViewModel);
        var tenantRequestListViewModel = typeof(TenantRequestListViewModel);
        var scheduleServiceWorkViewModel = typeof(ScheduleServiceWorkViewModel);

        // Assert all view model types exist
        submitTenantRequestViewModel.Should().NotBeNull();
        tenantRequestDetailsViewModel.Should().NotBeNull();
        tenantRequestListViewModel.Should().NotBeNull();
        scheduleServiceWorkViewModel.Should().NotBeNull();

        // Verify correct namespaces
        submitTenantRequestViewModel.Namespace.Should().Be("RentalRepairs.WebUI.Models");
        tenantRequestDetailsViewModel.Namespace.Should().Be("RentalRepairs.WebUI.Models");
        tenantRequestListViewModel.Namespace.Should().Be("RentalRepairs.WebUI.Models");
        scheduleServiceWorkViewModel.Namespace.Should().Be("RentalRepairs.WebUI.Models");
    }

    [Fact]
    public void Step14_All_Authentication_ViewModels_Exist()
    {
        // Validate that all required view models from Step 14 are created
        var loginViewModel = typeof(LoginViewModel);
        var tenantLoginViewModel = typeof(TenantLoginViewModel);
        var workerLoginViewModel = typeof(WorkerLoginViewModel);
        var dashboardViewModel = typeof(DashboardViewModel);

        // Assert all view model types exist
        loginViewModel.Should().NotBeNull();
        tenantLoginViewModel.Should().NotBeNull();
        workerLoginViewModel.Should().NotBeNull();
        dashboardViewModel.Should().NotBeNull();

        // Verify correct namespaces
        loginViewModel.Namespace.Should().Be("RentalRepairs.WebUI.Models");
        tenantLoginViewModel.Namespace.Should().Be("RentalRepairs.WebUI.Models");
        workerLoginViewModel.Namespace.Should().Be("RentalRepairs.WebUI.Models");
        dashboardViewModel.Namespace.Should().Be("RentalRepairs.WebUI.Models");
    }

    [Fact]
    public void Step14_RegisterPropertyViewModel_Has_Required_Properties()
    {
        // Test RegisterPropertyViewModel properties
        var viewModel = typeof(RegisterPropertyViewModel);
        var properties = viewModel.GetProperties().Select(p => p.Name).ToList();

        properties.Should().Contain("Name");
        properties.Should().Contain("Code");
        properties.Should().Contain("StreetAddress");
        properties.Should().Contain("City");
        properties.Should().Contain("State");
        properties.Should().Contain("ZipCode");
        properties.Should().Contain("SuperintendentFirstName");
        properties.Should().Contain("SuperintendentLastName");
        properties.Should().Contain("SuperintendentEmail");
        properties.Should().Contain("SuperintendentPhone");
    }

    [Fact]
    public void Step14_SubmitTenantRequestViewModel_Has_Required_Properties()
    {
        // Test SubmitTenantRequestViewModel properties
        var viewModel = typeof(SubmitTenantRequestViewModel);
        var properties = viewModel.GetProperties().Select(p => p.Name).ToList();

        properties.Should().Contain("PropertyCode");
        properties.Should().Contain("UnitNumber");
        properties.Should().Contain("TenantFirstName");
        properties.Should().Contain("TenantLastName");
        properties.Should().Contain("TenantEmail");
        properties.Should().Contain("TenantPhone");
        properties.Should().Contain("ProblemDescription");
        properties.Should().Contain("IsEmergency");
        properties.Should().Contain("PreferredContactTime");
    }

    [Fact]
    public void Step14_LoginViewModel_Has_Required_Properties()
    {
        // Test LoginViewModel properties
        var viewModel = typeof(LoginViewModel);
        var properties = viewModel.GetProperties().Select(p => p.Name).ToList();

        properties.Should().Contain("Email");
        properties.Should().Contain("Password");
        properties.Should().Contain("RememberMe");
        properties.Should().Contain("ReturnUrl");
    }

    [Fact]
    public void Step14_WebUI_Project_Structure_Validation()
    {
        // Validate project structure
        var webUIAssembly = typeof(Program).Assembly;
        webUIAssembly.Should().NotBeNull();

        // Verify key types exist
        var programType = typeof(Program);
        programType.Should().NotBeNull();

        var indexModelType = typeof(RentalRepairs.WebUI.Pages.IndexModel);
        indexModelType.Should().NotBeNull();

        var registerModelType = typeof(RentalRepairs.WebUI.Pages.Properties.RegisterModel);
        registerModelType.Should().NotBeNull();

        var submitModelType = typeof(RentalRepairs.WebUI.Pages.TenantRequests.SubmitModel);
        submitModelType.Should().NotBeNull();

        var loginModelType = typeof(RentalRepairs.WebUI.Pages.Account.LoginModel);
        loginModelType.Should().NotBeNull();
    }

    [Fact]
    public void Step14_Mapster_Config_Exists()
    {
        // Validate Mapster mapping configuration exists (not AutoMapper)
        var mappingConfigType = typeof(RentalRepairs.WebUI.Mappings.ApplicationToViewModelMappingConfig);
        mappingConfigType.Should().NotBeNull();
        mappingConfigType.Namespace.Should().Be("RentalRepairs.WebUI.Mappings");
    }
}