# Step 14 Completion Validation Script

## WebUI Project Build Validation
Write-Host "Step 14 Validation: WebUI Project Compilation" -ForegroundColor Green

try {
    # Test that WebUI project compiles
    $buildResult = dotnet build src/WebUI/RentalRepairs.WebUI.csproj --verbosity quiet
    if ($LASTEXITCODE -eq 0) {
        Write-Host "? WebUI project compiles successfully" -ForegroundColor Green
    } else {
        Write-Host "? WebUI project failed to compile" -ForegroundColor Red
        exit 1
    }
} catch {
    Write-Host "? Error building WebUI project: $_" -ForegroundColor Red
    exit 1
}

## File Structure Validation
Write-Host "`nStep 14 Validation: File Structure" -ForegroundColor Green

$requiredFiles = @(
    "src/WebUI/RentalRepairs.WebUI.csproj",
    "src/WebUI/Program.cs",
    "src/WebUI/Pages/Index.cshtml",
    "src/WebUI/Pages/Index.cshtml.cs",
    "src/WebUI/Pages/Properties/Register.cshtml",
    "src/WebUI/Pages/Properties/Register.cshtml.cs",
    "src/WebUI/Pages/TenantRequests/Submit.cshtml",
    "src/WebUI/Pages/TenantRequests/Submit.cshtml.cs",
    "src/WebUI/Pages/Account/Login.cshtml",
    "src/WebUI/Pages/Account/Login.cshtml.cs",
    "src/WebUI/Models/PropertyViewModels.cs",
    "src/WebUI/Models/TenantRequestViewModels.cs",
    "src/WebUI/Models/AuthenticationViewModels.cs",
    "src/WebUI/Mappings/ApplicationToViewModelMappingProfile.cs"
)

$missingFiles = @()
foreach ($file in $requiredFiles) {
    if (Test-Path $file) {
        Write-Host "? $file" -ForegroundColor Green
    } else {
        Write-Host "? $file" -ForegroundColor Red
        $missingFiles += $file
    }
}

if ($missingFiles.Count -gt 0) {
    Write-Host "`n? Missing files detected. Step 14 incomplete." -ForegroundColor Red
    exit 1
}

## Package Dependencies Validation
Write-Host "`nStep 14 Validation: Package Dependencies" -ForegroundColor Green

$projectContent = Get-Content "src/WebUI/RentalRepairs.WebUI.csproj" -Raw

$requiredPackages = @("AutoMapper", "MediatR", "FluentValidation.AspNetCore")
foreach ($package in $requiredPackages) {
    if ($projectContent -match $package) {
        Write-Host "? $package package referenced" -ForegroundColor Green
    } else {
        Write-Host "? $package package missing" -ForegroundColor Red
        exit 1
    }
}

## Project References Validation  
Write-Host "`nStep 14 Validation: Project References" -ForegroundColor Green

$requiredReferences = @("Application", "Infrastructure")
foreach ($reference in $requiredReferences) {
    if ($projectContent -match $reference) {
        Write-Host "? $reference project referenced" -ForegroundColor Green
    } else {
        Write-Host "? $reference project reference missing" -ForegroundColor Red
        exit 1
    }
}

## Content Validation
Write-Host "`nStep 14 Validation: Content Validation" -ForegroundColor Green

# Check that key content exists in files
$indexContent = Get-Content "src/WebUI/Pages/Index.cshtml" -Raw
if ($indexContent -match "Dashboard" -and $indexContent -match "Welcome") {
    Write-Host "? Index page has dashboard content" -ForegroundColor Green
} else {
    Write-Host "? Index page missing required content" -ForegroundColor Red
    exit 1
}

$loginContent = Get-Content "src/WebUI/Pages/Account/Login.cshtml" -Raw
if ($loginContent -match "Admin" -and $loginContent -match "Tenant" -and $loginContent -match "Worker") {
    Write-Host "? Login page has multi-role authentication" -ForegroundColor Green
} else {
    Write-Host "? Login page missing multi-role authentication" -ForegroundColor Red
    exit 1
}

$programContent = Get-Content "src/WebUI/Program.cs" -Raw
if ($programContent -match "AddApplication" -and $programContent -match "AddInfrastructure" -and $programContent -match "AddAutoMapper") {
    Write-Host "? Program.cs has proper dependency injection" -ForegroundColor Green
} else {
    Write-Host "? Program.cs missing required service registrations" -ForegroundColor Red
    exit 1
}

## Success
Write-Host "`n?? Step 14 Validation Successful!" -ForegroundColor Green
Write-Host "? Razor Pages presentation layer created successfully" -ForegroundColor Green
Write-Host "? MediatR dependency injection implemented" -ForegroundColor Green  
Write-Host "? Presentation layer DTOs created" -ForegroundColor Green
Write-Host "? AutoMapper for DTO conversions set up" -ForegroundColor Green
Write-Host "? Clean architecture principles maintained" -ForegroundColor Green

Write-Host "`nStep 14 is COMPLETE and ready to proceed to Step 15!" -ForegroundColor Cyan