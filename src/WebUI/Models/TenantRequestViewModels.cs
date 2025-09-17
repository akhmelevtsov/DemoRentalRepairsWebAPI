using System.ComponentModel.DataAnnotations;

namespace RentalRepairs.WebUI.Models;

/// <summary>
/// Presentation model for tenant request submission
/// </summary>
public class SubmitTenantRequestViewModel
{
    [Required(ErrorMessage = "Property code is required")]
    [Display(Name = "Property Code")]
    public string PropertyCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Unit number is required")]
    [Display(Name = "Unit Number")]
    [StringLength(20, ErrorMessage = "Unit number cannot exceed 20 characters")]
    public string UnitNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "First name is required")]
    [Display(Name = "First Name")]
    [StringLength(100, ErrorMessage = "First name cannot exceed 100 characters")]
    public string TenantFirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required")]
    [Display(Name = "Last Name")]
    [StringLength(100, ErrorMessage = "Last name cannot exceed 100 characters")]
    public string TenantLastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [Display(Name = "Email")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address")]
    public string TenantEmail { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone number is required")]
    [Display(Name = "Phone Number")]
    [Phone(ErrorMessage = "Please enter a valid phone number")]
    public string TenantPhone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Problem description is required")]
    [Display(Name = "Problem Description")]
    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    [MinLength(10, ErrorMessage = "Description must be at least 10 characters")]
    public string ProblemDescription { get; set; } = string.Empty;

    [Display(Name = "Is this an emergency?")]
    public bool IsEmergency { get; set; }

    [Display(Name = "Preferred Contact Time")]
    public string? PreferredContactTime { get; set; }
}

/// <summary>
/// Presentation model for tenant request details
/// </summary>
public class TenantRequestDetailsViewModel
{
    public int Id { get; set; }
    public string PropertyName { get; set; } = string.Empty;
    public string PropertyCode { get; set; } = string.Empty;
    public string UnitNumber { get; set; } = string.Empty;
    public string TenantName { get; set; } = string.Empty;
    public string TenantEmail { get; set; } = string.Empty;
    public string TenantPhone { get; set; } = string.Empty;
    public string ProblemDescription { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public bool IsEmergency { get; set; }
    public DateTime SubmittedDate { get; set; }
    public DateTime? ScheduledDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public string? WorkerName { get; set; }
    public string? WorkerEmail { get; set; }
    public string? CompletionNotes { get; set; }
    public string? PreferredContactTime { get; set; }
}

/// <summary>
/// Presentation model for tenant request list
/// </summary>
public class TenantRequestListViewModel
{
    public List<TenantRequestSummaryViewModel> Requests { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? StatusFilter { get; set; }
    public string? PropertyFilter { get; set; }
    public bool EmergencyOnly { get; set; }
    public string SortBy { get; set; } = "SubmittedDate";
    public string SortDirection { get; set; } = "desc";
}

/// <summary>
/// Presentation model for tenant request summary in list
/// </summary>
public class TenantRequestSummaryViewModel
{
    public int Id { get; set; }
    public string PropertyName { get; set; } = string.Empty;
    public string PropertyCode { get; set; } = string.Empty;
    public string UnitNumber { get; set; } = string.Empty;
    public string TenantName { get; set; } = string.Empty;
    public string ProblemDescription { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public bool IsEmergency { get; set; }
    public DateTime SubmittedDate { get; set; }
    public DateTime? ScheduledDate { get; set; }
}

/// <summary>
/// Presentation model for scheduling service work
/// </summary>
public class ScheduleServiceWorkViewModel
{
    public int RequestId { get; set; }
    public string PropertyName { get; set; } = string.Empty;
    public string UnitNumber { get; set; } = string.Empty;
    public string ProblemDescription { get; set; } = string.Empty;

    [Required(ErrorMessage = "Worker selection is required")]
    [Display(Name = "Assigned Worker")]
    public int WorkerId { get; set; }

    [Required(ErrorMessage = "Scheduled date is required")]
    [Display(Name = "Scheduled Date")]
    [DataType(DataType.DateTime)]
    public DateTime ScheduledDate { get; set; }

    [Display(Name = "Service Notes")]
    [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
    public string? ServiceNotes { get; set; }

    public List<WorkerOptionViewModel> AvailableWorkers { get; set; } = new();
}

/// <summary>
/// Presentation model for worker options in dropdowns
/// </summary>
public class WorkerOptionViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Specialization { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsAvailable { get; set; }
}