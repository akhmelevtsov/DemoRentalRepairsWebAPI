using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using RentalRepairs.Application;
using RentalRepairs.Infrastructure;
using RentalRepairs.WebUI.Mappings;
using RentalRepairs.WebUI.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog with basic configuration
builder.Host.UseSerilog((context, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .WriteTo.Console()
        .WriteTo.File("logs/rentalrepairs-.txt", rollingInterval: RollingInterval.Day)
        .Enrich.FromLogContext();
});

// Add services to the container
builder.Services.AddRazorPages(options =>
{
    // Configure Razor Pages routing and conventions
    options.Conventions.AuthorizeFolder("/", "RequireAuthenticatedUser");
    options.Conventions.AllowAnonymousToPage("/Account/Login");
    options.Conventions.AllowAnonymousToPage("/Privacy");
    options.Conventions.AllowAnonymousToPage("/Shared/Error");
});

// Add Clean Architecture layers
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Configure Mapster mappings
ApplicationToViewModelMappingConfig.RegisterMappings();

// Add Authentication & Authorization
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = builder.Environment.IsDevelopment() 
            ? CookieSecurePolicy.SameAsRequest 
            : CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.Name = "RentalRepairs.Auth";
        
        options.Events.OnRedirectToLogin = context =>
        {
            if (context.Request.Path.StartsWithSegments("/api"))
            {
                context.Response.StatusCode = 401;
                return Task.CompletedTask;
            }
            context.Response.Redirect(context.RedirectUri);
            return Task.CompletedTask;
        };
    });

// Configure Authorization Policies
builder.Services.AddAuthorization(options =>
{
    // Base authentication requirement
    options.AddPolicy("RequireAuthenticatedUser", policy =>
        policy.RequireAuthenticatedUser());

    // Role-based policies
    options.AddPolicy("RequireSystemAdminRole", policy =>
        policy.RequireClaim("role", "SystemAdmin"));

    options.AddPolicy("RequireSuperintendentRole", policy =>
        policy.RequireClaim("role", "PropertySuperintendent"));

    options.AddPolicy("RequireTenantRole", policy =>
        policy.RequireClaim("role", "Tenant"));

    options.AddPolicy("RequireWorkerRole", policy =>
        policy.RequireClaim("role", "Worker"));

    // Combined policies for flexibility
    options.AddPolicy("RequirePropertyManagementRole", policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim("role", "SystemAdmin") ||
            context.User.HasClaim("role", "PropertySuperintendent")));

    options.AddPolicy("RequireServiceRole", policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim("role", "SystemAdmin") ||
            context.User.HasClaim("role", "PropertySuperintendent") ||
            context.User.HasClaim("role", "Worker")));
});

// Add WebUI specific services
builder.Services.AddScoped<IViewRenderService, ViewRenderService>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

// Configure HTTPS redirection for production
if (!builder.Environment.IsDevelopment())
{
    builder.Services.AddHsts(options =>
    {
        options.Preload = true;
        options.IncludeSubDomains = true;
        options.MaxAge = TimeSpan.FromDays(365);
    });

    builder.Services.AddHttpsRedirection(options =>
    {
        options.RedirectStatusCode = StatusCodes.Status308PermanentRedirect;
        options.HttpsPort = 443;
    });
}

// Add basic health checks
builder.Services.AddHealthChecks()
    .AddCheck<RentalRepairs.WebUI.HealthChecks.ApplicationHealthCheck>("application");

// Configure response compression
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Shared/Error");
    app.UseHsts();
}

// Security headers middleware
app.Use(async (context, next) =>
{
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";
    context.Response.Headers["X-Frame-Options"] = "DENY";
    context.Response.Headers["X-XSS-Protection"] = "1; mode=block";
    context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
    
    if (!app.Environment.IsDevelopment())
    {
        context.Response.Headers["Content-Security-Policy"] = 
            "default-src 'self'; " +
            "script-src 'self' 'unsafe-inline' https://cdn.jsdelivr.net https://cdnjs.cloudflare.com; " +
            "style-src 'self' 'unsafe-inline' https://cdn.jsdelivr.net https://cdnjs.cloudflare.com; " +
            "font-src 'self' https://cdnjs.cloudflare.com; " +
            "img-src 'self' data: https:;";
    }
    
    await next.Invoke();
});

// Request logging
app.UseSerilogRequestLogging(options =>
{
    options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
    {
        diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
        diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
        
        if (httpContext.User.Identity?.IsAuthenticated == true)
        {
            diagnosticContext.Set("UserName", httpContext.User.Identity.Name);
            diagnosticContext.Set("UserRole", httpContext.User.FindFirst("role")?.Value ?? "Unknown");
        }
    };
});

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseResponseCompression();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Map health checks
app.MapHealthChecks("/health");

// Map Razor Pages
app.MapRazorPages();

// Configure default route with role-based redirection
app.MapGet("/", (HttpContext context) =>
{
    if (context.User.Identity?.IsAuthenticated == true)
    {
        return Results.Redirect("/Index");
    }
    else
    {
        return Results.Redirect("/Account/Login");
    }
});

// Global exception handling for API endpoints
app.Map("/api/{**path}", (HttpContext context) =>
{
    context.Response.StatusCode = 404;
    return Results.Json(new { error = "API endpoint not found", path = context.Request.Path });
});

// Ensure database is created and migrations are applied
using (var scope = app.Services.CreateScope())
{
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<RentalRepairs.Infrastructure.Persistence.ApplicationDbContext>();
        await context.Database.EnsureCreatedAsync();
        
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("Database initialization completed successfully");
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while initializing the database");
        
        // In development, continue; in production, you might want to stop
        if (!app.Environment.IsDevelopment())
        {
            throw;
        }
    }
}

app.Run();

// Make the implicit Program class public for testing
public partial class Program { }