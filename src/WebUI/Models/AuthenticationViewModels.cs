using System.ComponentModel.DataAnnotations;

namespace RentalRepairs.WebUI.Models;

/// <summary>
/// Presentation model for user authentication
/// </summary>
public class LoginViewModel
{
    [Required(ErrorMessage = "Email is required")]
    [Display(Name = "Email")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [Display(Name = "Password")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Remember me")]
    public bool RememberMe { get; set; }

    public string? ReturnUrl { get; set; }
}

/// <summary>
/// Presentation model for tenant authentication
/// </summary>
public class TenantLoginViewModel
{
    [Required(ErrorMessage = "Email is required")]
    [Display(Name = "Email")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Property code is required")]
    [Display(Name = "Property Code")]
    public string PropertyCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Unit number is required")]
    [Display(Name = "Unit Number")]
    public string UnitNumber { get; set; } = string.Empty;

    [Display(Name = "Remember me")]
    public bool RememberMe { get; set; }

    public string? ReturnUrl { get; set; }
}

/// <summary>
/// Presentation model for worker authentication
/// </summary>
public class WorkerLoginViewModel
{
    [Required(ErrorMessage = "Email is required")]
    [Display(Name = "Email")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Specialization is required")]
    [Display(Name = "Specialization")]
    public string Specialization { get; set; } = string.Empty;

    [Display(Name = "Remember me")]
    public bool RememberMe { get; set; }

    public string? ReturnUrl { get; set; }

    public List<string> AvailableSpecializations { get; set; } = new()
    {
        "Plumbing",
        "Electrical",
        "HVAC",
        "General Maintenance",
        "Carpentry",
        "Painting"
    };
}

/// <summary>
/// Presentation model for dashboard summary
/// </summary>
public class DashboardViewModel
{
    public UserRole UserRole { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;

    // Property superintendent dashboard
    public int TotalProperties { get; set; }
    public int TotalTenants { get; set; }
    public int PendingRequests { get; set; }
    public int EmergencyRequests { get; set; }
    public List<TenantRequestSummaryViewModel> RecentRequests { get; set; } = new();

    // Tenant dashboard
    public string? PropertyName { get; set; }
    public string? UnitNumber { get; set; }
    public List<TenantRequestSummaryViewModel> MyRequests { get; set; } = new();

    // Worker dashboard
    public int AssignedRequests { get; set; }
    public int CompletedThisMonth { get; set; }
    public List<TenantRequestSummaryViewModel> UpcomingWork { get; set; } = new();

    // System admin dashboard
    public int TotalSystemProperties { get; set; }
    public int TotalSystemRequests { get; set; }
    public int ActiveWorkers { get; set; }
    public List<PropertySummaryViewModel> RecentProperties { get; set; } = new();
}

/// <summary>
/// User roles for the presentation layer
/// </summary>
public enum UserRole
{
    SystemAdmin,
    PropertySuperintendent,
    Tenant,
    Worker
}

/// <summary>
/// Navigation menu item
/// </summary>
public class NavigationItem
{
    public string Title { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public List<NavigationItem> SubItems { get; set; } = new();
}