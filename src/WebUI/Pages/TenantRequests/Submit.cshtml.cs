using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RentalRepairs.Application.Commands.TenantRequests;
using RentalRepairs.WebUI.Models;
using Mapster;

namespace RentalRepairs.WebUI.Pages.TenantRequests;

public class SubmitModel : PageModel
{
    private readonly IMediator _mediator;
    private readonly ILogger<SubmitModel> _logger;

    public SubmitModel(IMediator mediator, ILogger<SubmitModel> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [BindProperty]
    public SubmitTenantRequestViewModel TenantRequest { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        // Pre-populate tenant information if user is authenticated as tenant
        if (User.Identity?.IsAuthenticated == true)
        {
            var tenantRole = User.FindFirst("role")?.Value;
            if (tenantRole == "Tenant")
            {
                TenantRequest.TenantEmail = User.FindFirst("email")?.Value ?? "";
                TenantRequest.PropertyCode = User.FindFirst("property_code")?.Value ?? "";
                TenantRequest.UnitNumber = User.FindFirst("unit_number")?.Value ?? "";
            }
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            var command = TenantRequest.Adapt<CreateTenantRequestCommand>();
            var requestId = await _mediator.Send(command);

            TempData["Success"] = "Your maintenance request has been submitted successfully. You will receive a confirmation email shortly.";
            return RedirectToPage("/TenantRequests/Details", new { id = requestId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error submitting tenant request for {TenantEmail}", TenantRequest.TenantEmail);
            ModelState.AddModelError(string.Empty, "An error occurred while submitting your request. Please try again.");
            return Page();
        }
    }
}