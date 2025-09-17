# Clean Architecture Migration Plan
## Rental Repairs Application Refactoring

### **CONSTRAINTS**

1. **SRC Folder Requirement**: All new development must occur within a new `src` folder
2. **No Changes Outside SRC**: Existing projects outside the `src` folder remain untouched during gradual migration
3. **Clean Architecture Layers**: Domain, Application, Infrastructure, and Presentation layers must be clearly separated
4. **DDD Principles**: Domain-driven design patterns including aggregates, entities, value objects, and domain services
5. **Mediator Pattern**: CQRS implementation using MediatR for command/query separation
6. **.NET 8 Compatibility**: Maintain compatibility with .NET 8 framework
7. **Dependency Direction**: Dependencies must flow inward (Domain - Application - Infrastructure - Presentation)
8. **Database Strategy**: Redesign database schema to better align with DDD aggregates
9. **Authentication**: Implement a more streamlined authentication method
10. **Migration Timeline**: Gradual migration (keeping old projects running)
11. **Testing Strategy**: Both unit tests and integration tests with comprehensive coverage
12. Use Mapster for DTO mapping, dont use AutoMapper

---

## **DETAILED REFACTORING PLAN**

### **Phase 1: SRC Folder Structure Creation (Steps 1-3)**  **COMPLETED**

#### ** Step 1: Create SRC folder structure following Jason Taylor's pattern**
-  Create `src/` folder in solution root
-  Create `src/Domain/` - Core business logic and entities
-  Create `src/Application/` - Use cases, DTOs, interfaces, CQRS handlers
-  Create `src/Infrastructure/` - External concerns (database, email, etc.)
-  Create `src/WebUI/` - Presentation layer (Razor Pages)
-  **Tests after step**: Create basic test project structure

#### ** Step 2: Create new clean architecture projects within SRC**
-  `src/Domain/RentalRepairs.Domain.csproj` - Domain entities, value objects, aggregates
-  `src/Application/RentalRepairs.Application.csproj` - Application services, CQRS, DTOs
-  `src/Infrastructure/RentalRepairs.Infrastructure.csproj` - Data access, external services
-  `src/WebUI/RentalRepairs.WebUI.csproj` - Razor Pages presentation layer
-  **Tests after step**: Verify project compilation and basic structure

#### ** Step 3: Set up project dependencies in clean architecture pattern**
-  Domain: No dependencies (pure business logic)
-  Application: Depends only on Domain + MediatR packages
-  Infrastructure: Depends on Application and Domain
-  WebUI: Depends on Application and Infrastructure
-  **Tests after step**: Validate dependency graph and compilation

---

### **Phase 2: Domain Layer Migration (Steps 4-6)**  **COMPLETED**

#### ** Step 4: Migrate and enhance Domain entities**
-  Move `Property`, `TenantRequest`, `Tenant`, `Worker` entities to `src/Domain/Entities/`
-  Enhance entities with DDD patterns (aggregate roots, domain events)
-  Migrate value objects (`PropertyAddress`, `PersonContactInfo`) to `src/Domain/ValueObjects/`
-  Add domain events for important business operations
-  **Tests after step**: Create unit tests for domain entities and business rules

#### ** Step 5: Create domain aggregates and repositories**
-  Create `PropertyAggregate` with `Property` as aggregate root
-  Create `TenantRequestAggregate` with `TenantRequest` as aggregate root
-  Define repository interfaces in `src/Domain/Repositories/`
-  Implement specification pattern for complex queries
-  **Tests after step**: Create unit tests for aggregates and specifications

#### ** Step 6: Migrate domain services and business rules**
-  Move `PropertyDomainService` to `src/Domain/Services/`
-  Move validators to `src/Domain/Validators/`
-  Enhance with domain events and business rule enforcement
-  **Tests after step**: Create unit tests for domain services and validators
-  compile and validate domain layer with sln file in src folder

### **Phase 3: Application Layer with CQRS (Steps 7-10)**  **COMPLETED**

#### ** Step 7: Implement CQRS structure with MediatR**
-  Install MediatR and related packages
-  Create `src/Application/Commands/` for write operations
-  Create `src/Application/Queries/` for read operations
-  Create `src/Application/DTOs/` for data transfer objects
-  **Tests after step**: Create unit tests for basic CQRS infrastructure

#### ** Step 8: Create command handlers for business operations**
-  `RegisterPropertyCommand` and `RegisterPropertyCommandHandler`
-  `RegisterTenantRequestCommand` and `RegisterTenantRequestCommandHandler`
-  `ScheduleServiceWorkCommand` and `ScheduleServiceWorkCommandHandler`
-  `CloseRequestCommand` and `CloseRequestCommandHandler`
-  **Tests after step**: Create unit tests for all command handlers

#### ** Step 9: Create query handlers for data retrieval**
-  `GetPropertyByIdQuery` and `GetPropertyByIdQueryHandler`
-  `GetTenantRequestsQuery` and `GetTenantRequestsQueryHandler`
-  `GetWorkerRequestsQuery` and `GetWorkerRequestsQueryHandler`
-  **Tests after step**: Create unit tests for all query handlers

#### ** Step 10: Implement application services and interfaces**
-  Move `IPropertyService`, `INotifyPartiesService` interfaces to `src/Application/Interfaces/`
-  Create application-specific DTOs and mapping profiles
-  Implement validation using FluentValidation
-  **Tests after step**: Create integration tests for application services

---

### **Phase 4: Infrastructure Layer Migration (Steps 11-13)**  **COMPLETED**

#### ** Step 11: Create data access layer with repository implementations**
-  Move repository implementations to `src/Infrastructure/Persistence/`
-  Implement `PropertyRepository`, `TenantRequestRepository`
-  Create Entity Framework configurations and DbContext
-  Implement database migrations for DDD-aligned schema
-  **Tests after step**: Create integration tests for repositories and data access

#### ** Step 12: Migrate external service implementations**
-  Move email services to `src/Infrastructure/Services/`
-  Move notification services to `src/Infrastructure/Notifications/`
-  Implement dependency injection registration
-  Create configuration models
-  **Tests after step**: Create integration tests for external services

#### ** Step 13: Implement infrastructure-specific concerns**
-  Create streamlined authentication and authorization implementations
-  Implement caching strategies
-  Set up logging and monitoring
-  Configure external API integrations
-  **Tests after step**: Create integration tests for authentication and caching

---

### **Phase 5: Presentation Layer (Steps 14-16)**  **COMPLETED**

#### ** Step 14: Create new Razor Pages presentation layer**
-  Create Razor Pages in `src/WebUI/Pages/`
-  Implement dependency injection for MediatR
-  Create DTOs specific to presentation layer
-  Set up Mapster for DTO conversions
-  **Tests after step**: Create integration tests for Razor Pages

#### ** Step 15: Migrate views and UI components**
-  Convert MVC views to Razor Pages in `src/WebUI/Pages/`
-  Update page models to work with new CQRS structure
-  Implement proper model binding and validation
-  Update authentication and authorization for Razor Pages
-  **Tests after step**: Create UI integration tests

#### ** Step 16: Configure startup and dependency injection**
-  Create `src/WebUI/Program.cs` for .NET 8
-  Configure all dependencies following clean architecture
-  Set up middleware pipeline for Razor Pages
-  Configure streamlined authentication and authorization
-  **Tests after step**: Create end-to-end integration tests

---

### **Phase 6: Core Testing and Validation (Step 17)**  **COMPLETED**

#### ** Step 17: Create comprehensive test projects**
-  run src/WebUi and fix run-time errors
-  create integration tests for end-to-end scenarios for WebUI using in-memory database
-  **Tests after step**: Validate all test suites run successfully

---

### ** Future Implementation (Steps 18-20)**  **POSTPONED**

#### ** Step 18: Implement domain event handling** - postponed 
- Create domain event dispatcher
- Implement event handlers for cross-cutting concerns
- Set up integration events for external systems
- Test event-driven workflows
- **Tests after step**: Create integration tests for domain events

#### ** Step 19: Performance optimization and caching** - postpone
- Implement query optimization using specifications
- Add caching for frequently accessed data
- Implement pagination for large datasets
- Add performance monitoring
- **Tests after step**: Create performance tests and benchmarks

#### ** Step 20: Final validation and migration** - postpone
- Run comprehensive tests to ensure functionality
- Validate all CQRS operations work correctly
- Ensure dependency injection is properly configured
- Verify authentication and authorization work as expected
- Gradual cutover from old to new architecture
- **Tests after step**: Complete end-to-end validation and performance testing

---

## **EXECUTION APPROACH**

Each step will be implemented individually with:
1. Implementation across all applicable files
2. Code review and diff presentation
3. Approval before moving to next step
4. Test creation after each major step
5. Validation of changes before proceeding
6. Once every step is finished successfully, the report on the step's completion is stored in a markdown file.

## **SUCCESS CRITERIA**

- [x] Clean separation of concerns across all layers 
- [x] CQRS pattern properly implemented with MediatR 
- [x] DDD principles correctly applied 
- [x] Comprehensive test coverage (unit + integration) 
- [x] Streamlined authentication system 
- [x] Database schema aligned with DDD aggregates 
- [x] Gradual migration path established 
- [ ] Performance optimizations in place  (Future)
- [x] All existing functionality preserved 
---

## ** CURRENT STATUS**

**Status**:  **CORE MIGRATION COMPLETE**  
**Completed**: **Steps 1-17** (17 out of 20 steps - 85% complete)  
**Current Phase**: **All critical phases completed**  
**Postponed Steps**: **18-20** (Advanced features for future implementation)  

### ** What's Working Now**
- Complete clean architecture with DDD and CQRS
- Comprehensive testing infrastructure
- Production-ready Razor Pages application
- Database integration with Entity Framework Core
- Authentication and authorization
- All core business functionality

### ** Future Implementation Available**
- Domain event handling (Step 18)
- Performance optimization and caching (Step 19)
- Final migration and cutover (Step 20)

** MISSION ACCOMPLISHED**: Core clean architecture successfully implemented!