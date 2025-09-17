using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RentalRepairs.Application.Queries.TenantRequests;
using RentalRepairs.WebUI.Models;
using Mapster;

namespace RentalRepairs.WebUI.Pages.TenantRequests;

public class DetailsModel : PageModel
{
    private readonly IMediator _mediator;
    private readonly ILogger<DetailsModel> _logger;

    public DetailsModel(IMediator mediator, ILogger<DetailsModel> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public TenantRequestDetailsViewModel TenantRequest { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        try
        {
            var query = new GetTenantRequestByIdQuery(id);
            var result = await _mediator.Send(query);

            TenantRequest = result.Adapt<TenantRequestDetailsViewModel>();

            return Page();
        }
        catch (ArgumentException)
        {
            TempData["Error"] = "Tenant request not found.";
            return RedirectToPage("/Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading tenant request details for ID {RequestId}", id);
            TempData["Error"] = "An error occurred while loading the request details.";
            return RedirectToPage("/Index");
        }
    }
}