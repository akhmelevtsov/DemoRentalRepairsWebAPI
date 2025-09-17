using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RentalRepairs.Application.Queries.Properties;
using RentalRepairs.Application.Queries.TenantRequests;
using RentalRepairs.WebUI.Models;
using Mapster;

namespace RentalRepairs.WebUI.Pages;

public class IndexModel : PageModel
{
    private readonly IMediator _mediator;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(IMediator mediator, ILogger<IndexModel> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public DashboardViewModel Dashboard { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        try
        {
            await LoadDashboardDataAsync();
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading dashboard data");
            TempData["Error"] = "Unable to load dashboard data. Please try again.";
            return Page();
        }
    }

    private async Task LoadDashboardDataAsync()
    {
        // Determine user role from claims (simplified for demo)
        var userRole = GetUserRoleFromClaims();
        Dashboard.UserRole = userRole;
        Dashboard.UserName = User.Identity?.Name ?? "Guest";
        Dashboard.UserEmail = User.FindFirst("email")?.Value ?? "";

        switch (userRole)
        {
            case UserRole.SystemAdmin:
                await LoadSystemAdminDashboardAsync();
                break;
            case UserRole.PropertySuperintendent:
                await LoadPropertySuperintendentDashboardAsync();
                break;
            case UserRole.Tenant:
                await LoadTenantDashboardAsync();
                break;
            case UserRole.Worker:
                await LoadWorkerDashboardAsync();
                break;
        }
    }

    private async Task LoadSystemAdminDashboardAsync()
    {
        // Get system-wide statistics
        var propertiesQuery = new GetPropertiesQuery { PageSize = 5, PageNumber = 1 };
        var propertiesResult = await _mediator.Send(propertiesQuery);

        var requestsQuery = new GetTenantRequestsQuery { PageSize = 10, PageNumber = 1 };
        var requestsResult = await _mediator.Send(requestsQuery);

        Dashboard.TotalSystemProperties = propertiesResult.TotalCount;
        Dashboard.TotalSystemRequests = requestsResult.TotalCount;
        Dashboard.RecentProperties = propertiesResult.Items.Adapt<List<PropertySummaryViewModel>>();
        Dashboard.RecentRequests = requestsResult.Items.Adapt<List<TenantRequestSummaryViewModel>>();
    }

    private async Task LoadPropertySuperintendentDashboardAsync()
    {
        // Get superintendent-specific data
        var userEmail = Dashboard.UserEmail;
        
        var requestsQuery = new GetTenantRequestsQuery 
        { 
            PageSize = 10, 
            PageNumber = 1
            // Note: SuperintendentEmail filtering needs to be handled in the query handler
        };
        var requestsResult = await _mediator.Send(requestsQuery);

        Dashboard.PendingRequests = requestsResult.Items.Count(r => r.Status.ToString() == "Submitted");
        Dashboard.EmergencyRequests = requestsResult.Items.Count(r => r.UrgencyLevel == "High");
        Dashboard.RecentRequests = requestsResult.Items.Adapt<List<TenantRequestSummaryViewModel>>();
    }

    private async Task LoadTenantDashboardAsync()
    {
        // Get tenant-specific data
        var tenantIdClaim = User.FindFirst("tenant_id")?.Value;
        if (int.TryParse(tenantIdClaim, out var tenantId))
        {
            var requestsQuery = new GetTenantRequestsQuery 
            { 
                TenantId = tenantId,
                PageSize = 10,
                PageNumber = 1
            };
            var requestsResult = await _mediator.Send(requestsQuery);

            Dashboard.PropertyName = User.FindFirst("property_name")?.Value;
            Dashboard.UnitNumber = User.FindFirst("unit_number")?.Value;
            Dashboard.MyRequests = requestsResult.Items.Adapt<List<TenantRequestSummaryViewModel>>();
        }
    }

    private async Task LoadWorkerDashboardAsync()
    {
        // Get worker-specific data
        var workerEmail = User.FindFirst("email")?.Value;
        if (!string.IsNullOrEmpty(workerEmail))
        {
            var requestsQuery = new GetTenantRequestsQuery 
            { 
                WorkerEmail = workerEmail,
                PageSize = 10,
                PageNumber = 1
            };
            var requestsResult = await _mediator.Send(requestsQuery);

            Dashboard.AssignedRequests = requestsResult.TotalCount;
            Dashboard.CompletedThisMonth = requestsResult.Items.Count(r => 
                r.Status.ToString() == "Completed" && r.CompletedDate?.Month == DateTime.Now.Month);
            Dashboard.UpcomingWork = requestsResult.Items
                .Where(r => r.ScheduledDate.HasValue && r.ScheduledDate > DateTime.Now)
                .Adapt<List<TenantRequestSummaryViewModel>>();
        }
    }

    private UserRole GetUserRoleFromClaims()
    {
        var roleClaim = User.FindFirst("role")?.Value;
        return roleClaim switch
        {
            "SystemAdmin" => UserRole.SystemAdmin,
            "PropertySuperintendent" => UserRole.PropertySuperintendent,
            "Tenant" => UserRole.Tenant,
            "Worker" => UserRole.Worker,
            _ => UserRole.Tenant // Default
        };
    }
}