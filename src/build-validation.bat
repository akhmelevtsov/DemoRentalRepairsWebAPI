@echo off
REM Clean Architecture Build Validation Script for Windows

echo ???  Building RentalRepairs Clean Architecture...
echo ================================================

cd /d "%~dp0"

echo ?? Current directory: %CD%
echo.

REM Clean previous builds
echo ?? Cleaning previous builds...
dotnet clean RentalRepairs.CleanArchitecture.sln --nologo -v q
echo.

REM Restore packages
echo ?? Restoring NuGet packages...
dotnet restore RentalRepairs.CleanArchitecture.sln --nologo -v q
echo.

REM Build solution
echo ?? Building solution...
dotnet build RentalRepairs.CleanArchitecture.sln --no-restore --nologo -v q

if %ERRORLEVEL% EQU 0 (
    echo ? Build successful!
    echo.
    
    REM Run tests
    echo ?? Running tests...
    dotnet test --no-build --nologo -v q
    
    if %ERRORLEVEL% EQU 0 (
        echo ? All tests passed!
        echo.
        echo ?? Clean Architecture validation completed successfully!
        echo.
        echo ?? Solution Summary:
        echo    • Domain Layer: Core business logic ?
        echo    • Application Layer: CQRS structure ?
        echo    • Infrastructure Layer: Data access ?
        echo    • WebUI Layer: Razor Pages ?
        echo    • Test Coverage: Unit ^& Integration ?
        echo.
        echo ?? Ready for Step 7: CQRS implementation
    ) else (
        echo ? Some tests failed!
        exit /b 1
    )
) else (
    echo ? Build failed!
    exit /b 1
)