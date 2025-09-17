using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RentalRepairs.Application.Commands.Properties;
using RentalRepairs.WebUI.Models;
using Mapster;

namespace RentalRepairs.WebUI.Pages.Properties;

public class RegisterModel : PageModel
{
    private readonly IMediator _mediator;
    private readonly ILogger<RegisterModel> _logger;

    public RegisterModel(IMediator mediator, ILogger<RegisterModel> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [BindProperty]
    public RegisterPropertyViewModel Property { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
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
            var command = Property.Adapt<RegisterPropertyCommand>();
            var propertyId = await _mediator.Send(command);

            TempData["Success"] = $"Property '{Property.Name}' has been registered successfully.";
            return RedirectToPage("/Properties/Details", new { id = propertyId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering property {PropertyName}", Property.Name);
            ModelState.AddModelError(string.Empty, "An error occurred while registering the property. Please try again.");
            return Page();
        }
    }
}