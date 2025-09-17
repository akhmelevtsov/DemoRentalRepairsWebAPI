# Rental Repairs Modernization Project

This repository demonstrates a **systematic AI-assisted modernization** of a legacy .NET application into a clean, maintainable, and extensible solution. The project highlights **Clean Architecture**, **Domain-Driven Design (DDD)**, **CQRS**, and **MediatR**, with GitHub Copilot in agent mode assisting the process.

---

## 🧭 Repository Structure

- `legacy/` — Original codebase (preserved for reference and comparison)
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

- [`docs/domain-overview.md`](docs/domain-overview.md) — Domain model concepts  
- [`docs/migration-process.md`](docs/migration-process.md) — Migration strategy and execution plan  
- [`docs/architecture-comparison.md`](docs/architecture-comparison.md) — Benefits of the new architecture  

---

## 🚀 Getting Started

```bash
cd src/WebUI
dotnet run