## Rental Repairs Modernization Project

This repository demonstrates a **systematic modernization** of a legacy .NET application into a clean, maintainable, and extensible solution. The project highlights **Clean Architecture**, **Domain-Driven Design (DDD)**, **CQRS**, and **MediatR**, with GitHub Copilot in agent mode assisting the process.

---

## 🧭 Repository Structure

- `/` — Original codebase (preserved for reference and comparison)
- `src/` — Modernized solution following Clean Architecture:
  - `Domain/` — Core business logic, entities, value objects, domain events
  - `Application/` — Use cases, CQRS handlers, DTOs, validation
  - `Infrastructure/` — EF Core persistence, external services, repositories
  - `WebUI/` — Razor Pages presentation layer

---

## 🛠 Technologies

- **.NET 8**
- **MediatR** for CQRS
- **Entity Framework Core** for persistence
- **Mapster** for DTO mapping
- **FluentValidation** for input validation
- **xUnit** for testing

---

## 🎯 Objectives

1. Demonstrate a **step-by-step migration process** using AI with human-in-the-loop approvals  
2. Apply **Clean Architecture** and **DDD** principles to a legacy project  
3. Showcase the **benefits of the new architecture** compared to the original  

---

## 📄 Documentation

- [`docs/domain-overview.md`](docs/domain-overview.md) — Business domain concepts, entities, and workflows  
- [`docs/migration-process.md`](docs/migration-process.md) — Migration strategy and execution plan  
- [`docs/architecture-comparison.md`](docs/architecture-comparison.md) — Benefits of the new architecture  

---

## 🚀 Getting Started

### Prerequisites

Before running the application, ensure you have the following installed:

- **[.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)** (8.0 or later)
- **SQL Server** or **SQL Server Express** ([Download here](https://www.microsoft.com/en-us/sql-server/sql-server-downloads))
  - Alternative: **SQL Server LocalDB** (included with Visual Studio)
- **Visual Studio 2022** (recommended) or **Visual Studio Code**
- **Git** for cloning the repository

### Quick Start (Modernized Application)

1. **Clone the repository**
   ```bash
   git clone https://github.com/akhmelevtsov/DemoRentalRepairsWebAPI.git
   cd DemoRentalRepairsWebAPI
   ```

2. **Navigate to the modernized solution**
   ```bash
   cd src
   ```

3. **Restore NuGet packages**
   ```bash
   dotnet restore
   ```

4. **Update database connection string** (if needed)
   
   Edit `src/WebUI/appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=RentalRepairsDb;Trusted_Connection=true;MultipleActiveResultSets=true;"
     }
   }
   ```
   
   For SQL Server Express, use:
   ```json
   "DefaultConnection": "Server=.\\SQLEXPRESS;Database=RentalRepairsDb;Trusted_Connection=true;MultipleActiveResultSets=true;"
   ```

5. **Run the application**
   ```bash
   cd WebUI
   dotnet run
   ```

6. **Open your browser**
   
   Navigate to: `https://localhost:5001` or `http://localhost:5000`

### First Run Setup

The application will automatically:
- ✅ Create the database if it doesn't exist
- ✅ Apply any pending migrations
- ✅ Log initialization status to console

### Default Test Data

To get started quickly, you can register test users:

1. **Property Manager (Superintendent)**
   - Click "Register" and create an account
   - Register a property with sample data:
     - Property Code: `test-property`
     - Units: `101, 102, 103, 201, 202, 203`

2. **Tenant**
   - Register as a tenant for unit `101` in `test-property`
   - Submit repair requests to test the workflow

3. **Worker**
   - Register as a worker to receive work assignments

### Running Legacy Version (Comparison)

To compare with the original legacy implementation:

1. **Navigate to legacy WebMVC**
   ```bash
   cd Demo.RentalRepairs.WebMvc
   dotnet run
   ```

2. **Navigate to legacy WebAPI**
   ```bash
   cd Demo.RentalRepairs.WebApi
   dotnet run
   ```

### Running Tests

**Run all tests:**
```bash
cd src
dotnet test
```

**Run specific test projects:**
```bash
# Domain tests
dotnet test Domain.Tests/

# Application tests  
dotnet test Application.Tests/

# Integration tests
dotnet test WebUI.Tests/
```

**Generate test coverage report:**
```bash
dotnet test --collect:"XPlat Code Coverage"
```

### Database Management

**Reset database:**
```bash
cd src/WebUI
dotnet ef database drop --force
dotnet run  # Will recreate on startup
```

**Apply migrations manually:**
```bash
cd src/Infrastructure
dotnet ef database update --startup-project ../WebUI
```

### Troubleshooting

#### Common Issues

**1. Database Connection Errors**
- Verify SQL Server is running
- Check connection string in `appsettings.json`
- Ensure database permissions are correct

**2. Port Already in Use**
```bash
# Check what's using the port
netstat -ano | findstr :5001

# Kill the process (replace PID)
taskkill /PID <process_id> /F
```

**3. SSL Certificate Issues**
```bash
# Trust the development certificate
dotnet dev-certs https --trust
```

**4. Build Errors**
```bash
# Clean and rebuild
dotnet clean
dotnet restore
dotnet build
```

#### Enable Detailed Logging

Add to `appsettings.Development.json`:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.EntityFrameworkCore": "Information"
    }
  }
}
```

### Configuration Options

The application supports various configuration options in `appsettings.json`:

- **Caching**: Enable/disable in-memory caching
- **Email Notifications**: Configure SMTP or mock email provider  
- **Logging**: Serilog configuration for file and console output
- **Security**: Authentication and authorization settings

### What's Working Now

✅ **Complete Clean Architecture Implementation**
- Domain layer with DDD patterns
- Application layer with CQRS and MediatR
- Infrastructure layer with EF Core
- Presentation layer with Razor Pages

✅ **Core Business Functionality**
- Property registration and management
- Tenant request submission and tracking
- Worker assignment and task completion
- User authentication and authorization

✅ **Comprehensive Testing**
- 50+ unit and integration tests
- Domain logic validation
- End-to-end workflow testing

✅ **Production-Ready Features**
- Security headers and authentication
- Logging and health checks
- Error handling and validation
- Performance optimizations

---







