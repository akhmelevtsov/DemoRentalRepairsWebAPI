using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RentalRepairs.WebUI.Models;
using System.Security.Claims;
using CustomAuth = RentalRepairs.Infrastructure.Authentication;

namespace RentalRepairs.WebUI.Pages.Account;

public class LoginModel : PageModel
{
    private readonly CustomAuth.IAuthenticationService _authService;
    private readonly ILogger<LoginModel> _logger;

    public LoginModel(CustomAuth.IAuthenticationService authService, ILogger<LoginModel> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [BindProperty]
    public LoginViewModel Login { get; set; } = new();

    [BindProperty]
    public TenantLoginViewModel TenantLogin { get; set; } = new();

    [BindProperty]
    public WorkerLoginViewModel WorkerLogin { get; set; } = new();

    public string ActiveTab { get; set; } = "admin";

    public async Task<IActionResult> OnGetAsync(string? returnUrl = null, string? tab = null)
    {
        if (!string.IsNullOrEmpty(tab))
        {
            ActiveTab = tab;
        }

        Login.ReturnUrl = returnUrl;
        TenantLogin.ReturnUrl = returnUrl;
        WorkerLogin.ReturnUrl = returnUrl;

        // Clear the existing external cookie to ensure a clean login process
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        return Page();
    }

    public async Task<IActionResult> OnPostAdminLoginAsync()
    {
        ActiveTab = "admin";

        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            var result = await _authService.AuthenticateAsync(Login.Email, Login.Password);

            if (result.IsSuccess)
            {
                var claims = new List<Claim>
                {
                    new(ClaimTypes.NameIdentifier, result.UserId),
                    new(ClaimTypes.Name, result.DisplayName),
                    new(ClaimTypes.Email, result.Email),
                    new("role", result.Roles.FirstOrDefault() ?? "SystemAdmin")
                };

                // Add custom claims
                foreach (var claim in result.Claims)
                {
                    claims.Add(new Claim(claim.Key, claim.Value.ToString()));
                }

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = Login.RememberMe,
                    ExpiresUtc = result.ExpiresAt
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                _logger.LogInformation("User {Email} logged in successfully", Login.Email);

                return LocalRedirect(Login.ReturnUrl ?? "/");
            }
            else
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Invalid login attempt.");
                return Page();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during admin login for {Email}", Login.Email);
            ModelState.AddModelError(string.Empty, "An error occurred during login. Please try again.");
            return Page();
        }
    }

    public async Task<IActionResult> OnPostTenantLoginAsync()
    {
        ActiveTab = "tenant";

        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            var result = await _authService.AuthenticateTenantAsync(
                TenantLogin.Email, 
                TenantLogin.PropertyCode, 
                TenantLogin.UnitNumber);

            if (result.IsSuccess)
            {
                var claims = new List<Claim>
                {
                    new(ClaimTypes.NameIdentifier, result.UserId),
                    new(ClaimTypes.Name, result.DisplayName),
                    new(ClaimTypes.Email, result.Email),
                    new("role", "Tenant")
                };

                // Add custom claims
                foreach (var claim in result.Claims)
                {
                    claims.Add(new Claim(claim.Key, claim.Value.ToString()));
                }

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = TenantLogin.RememberMe,
                    ExpiresUtc = result.ExpiresAt
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                _logger.LogInformation("Tenant {Email} logged in successfully", TenantLogin.Email);

                return LocalRedirect(TenantLogin.ReturnUrl ?? "/");
            }
            else
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Invalid tenant login credentials.");
                return Page();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during tenant login for {Email}", TenantLogin.Email);
            ModelState.AddModelError(string.Empty, "An error occurred during login. Please try again.");
            return Page();
        }
    }

    public async Task<IActionResult> OnPostWorkerLoginAsync()
    {
        ActiveTab = "worker";

        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            var result = await _authService.AuthenticateWorkerAsync(
                WorkerLogin.Email, 
                WorkerLogin.Specialization);

            if (result.IsSuccess)
            {
                var claims = new List<Claim>
                {
                    new(ClaimTypes.NameIdentifier, result.UserId),
                    new(ClaimTypes.Name, result.DisplayName),
                    new(ClaimTypes.Email, result.Email),
                    new("role", "Worker")
                };

                // Add custom claims
                foreach (var claim in result.Claims)
                {
                    claims.Add(new Claim(claim.Key, claim.Value.ToString()));
                }

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = WorkerLogin.RememberMe,
                    ExpiresUtc = result.ExpiresAt
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                _logger.LogInformation("Worker {Email} logged in successfully", WorkerLogin.Email);

                return LocalRedirect(WorkerLogin.ReturnUrl ?? "/");
            }
            else
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage ?? "Invalid worker login credentials.");
                return Page();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during worker login for {Email}", WorkerLogin.Email);
            ModelState.AddModelError(string.Empty, "An error occurred during login. Please try again.");
            return Page();
        }
    }
}