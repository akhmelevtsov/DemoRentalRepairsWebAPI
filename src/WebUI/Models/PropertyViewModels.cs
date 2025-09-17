using System.ComponentModel.DataAnnotations;

namespace RentalRepairs.WebUI.Models;

/// <summary>
/// Presentation model for property registration
/// </summary>
public class RegisterPropertyViewModel
{
    [Required(ErrorMessage = "Property name is required")]
    [Display(Name = "Property Name")]
    [StringLength(200, ErrorMessage = "Property name cannot exceed 200 characters")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Property code is required")]
    [Display(Name = "Property Code")]
    [StringLength(20, ErrorMessage = "Property code cannot exceed 20 characters")]
    public string Code { get; set; } = string.Empty;

    [Required(ErrorMessage = "Street address is required")]
    [Display(Name = "Street Address")]
    [StringLength(200, ErrorMessage = "Street address cannot exceed 200 characters")]
    public string StreetAddress { get; set; } = string.Empty;

    [Required(ErrorMessage = "City is required")]
    [Display(Name = "City")]
    [StringLength(100, ErrorMessage = "City cannot exceed 100 characters")]
    public string City { get; set; } = string.Empty;

    [Required(ErrorMessage = "State is required")]
    [Display(Name = "State")]
    [StringLength(50, ErrorMessage = "State cannot exceed 50 characters")]
    public string State { get; set; } = string.Empty;

    [Required(ErrorMessage = "ZIP code is required")]
    [Display(Name = "ZIP Code")]
    [StringLength(20, ErrorMessage = "ZIP code cannot exceed 20 characters")]
    public string ZipCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Superintendent first name is required")]
    [Display(Name = "Superintendent First Name")]
    [StringLength(100, ErrorMessage = "First name cannot exceed 100 characters")]
    public string SuperintendentFirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Superintendent last name is required")]
    [Display(Name = "Superintendent Last Name")]
    [StringLength(100, ErrorMessage = "Last name cannot exceed 100 characters")]
    public string SuperintendentLastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Superintendent email is required")]
    [Display(Name = "Superintendent Email")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address")]
    [StringLength(200, ErrorMessage = "Email cannot exceed 200 characters")]
    public string SuperintendentEmail { get; set; } = string.Empty;

    [Required(ErrorMessage = "Superintendent phone is required")]
    [Display(Name = "Superintendent Phone")]
    [Phone(ErrorMessage = "Please enter a valid phone number")]
    [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
    public string SuperintendentPhone { get; set; } = string.Empty;
}

/// <summary>
/// Presentation model for property details view
/// </summary>
public class PropertyDetailsViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string FullAddress { get; set; } = string.Empty;
    public string SuperintendentName { get; set; } = string.Empty;
    public string SuperintendentEmail { get; set; } = string.Empty;
    public string SuperintendentPhone { get; set; } = string.Empty;
    public int TenantCount { get; set; }
    public int ActiveRequestsCount { get; set; }
    public DateTime CreatedDate { get; set; }
}

/// <summary>
/// Presentation model for property list view
/// </summary>
public class PropertyListViewModel
{
    public List<PropertySummaryViewModel> Properties { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string SearchTerm { get; set; } = string.Empty;
    public string SortBy { get; set; } = "Name";
    public string SortDirection { get; set; } = "asc";
}

/// <summary>
/// Presentation model for property summary in list
/// </summary>
public class PropertySummaryViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public int TenantCount { get; set; }
    public int ActiveRequestsCount { get; set; }
    public string SuperintendentName { get; set; } = string.Empty;
}