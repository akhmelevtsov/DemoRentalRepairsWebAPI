#!/bin/bash

# Clean Architecture Build Validation Script
echo "???  Building RentalRepairs Clean Architecture..."
echo "================================================"

# Navigate to src directory
cd "$(dirname "$0")"

echo "?? Current directory: $(pwd)"
echo ""

# Clean previous builds
echo "?? Cleaning previous builds..."
dotnet clean RentalRepairs.CleanArchitecture.sln --nologo -v q
echo ""

# Restore packages
echo "?? Restoring NuGet packages..."
dotnet restore RentalRepairs.CleanArchitecture.sln --nologo -v q
echo ""

# Build solution
echo "?? Building solution..."
dotnet build RentalRepairs.CleanArchitecture.sln --no-restore --nologo -v q

if [ $? -eq 0 ]; then
    echo "? Build successful!"
    echo ""
    
    # Run tests
    echo "?? Running tests..."
    dotnet test --no-build --nologo -v q
    
    if [ $? -eq 0 ]; then
        echo "? All tests passed!"
        echo ""
        echo "?? Clean Architecture validation completed successfully!"
        echo ""
        echo "?? Solution Summary:"
        echo "   • Domain Layer: Core business logic ?"
        echo "   • Application Layer: CQRS structure ?"
        echo "   • Infrastructure Layer: Data access ?"
        echo "   • WebUI Layer: Razor Pages ?"
        echo "   • Test Coverage: Unit & Integration ?"
        echo ""
        echo "?? Ready for Step 7: CQRS implementation"
    else
        echo "? Some tests failed!"
        exit 1
    fi
else
    echo "? Build failed!"
    exit 1
fi