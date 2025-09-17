# Domain Overview

This document describes the **business domain** of the Rental Repairs system. It captures the core concepts, responsibilities, and workflows that the software supports, independent of any technical implementation.

> **📋 Real-World Context**: This domain model closely mirrors actual rental property management systems found in today's market, including platforms like AppFolio, Buildium, RentManager, and similar property management software solutions. The business processes, entity relationships, and workflow patterns reflect industry-standard practices used by real property management companies and maintenance coordination services.

---

## 🏠 Core Purpose

The Rental Repairs system manages the lifecycle of **maintenance requests** for rental properties. It connects **tenants**, **property managers**, and **workers** to ensure that repair issues are reported, tracked, and resolved efficiently.

---

## 👥 Key Actors

- **Tenant**  
  - Lives in a rental property.  
  - Submits repair requests when issues arise.  
  - Tracks the status of their requests.  

- **Property Manager**  
  - Oversees one or more properties.  
  - Reviews incoming tenant requests.  
  - Assigns requests to appropriate workers.  
  - Communicates updates to tenants.  

- **Worker (Contractor/Staff)**  
  - Receives assigned repair tasks.  
  - Performs the repair work.  
  - Updates the system with progress and completion details.  

---

## 🏛 Core Entities

- **Property**  
  - Represents a rental unit or building.  
  - Contains details such as address, unit number, and manager.  

- **Tenant Request**  
  - A repair or maintenance issue reported by a tenant.  
  - Includes description, urgency, and current status.  
  - Moves through a lifecycle: *Created → Assigned → In Progress → Completed/Closed*.  

- **Assignment**  
  - Links a repair request to a worker.  
  - Tracks scheduling, acceptance, and completion.  

---

## 💎 Value Objects

- **PropertyAddress**  
  - Encapsulates complete address information (street number, street name, city, postal code).  
  - Provides formatted address display and validation logic.  

- **PersonContactInfo**  
  - Contains personal identification and communication details (first name, last name, email, phone).  
  - Ensures email format validation and provides full name formatting.  

- **RequestPriority**  
  - Captures urgency level and priority scoring for repair requests.  
  - Enables intelligent prioritization based on safety and urgency factors.  

---

## 🔄 Business Processes

1. **Request Submission**  
   - Tenant submits a repair request for their property.  

2. **Review & Assignment**  
   - Property manager reviews the request.  
   - Manager assigns the request to a worker based on availability and skills.  

3. **Repair Execution**  
   - Worker performs the repair.  
   - Worker updates progress and marks completion.  

4. **Closure & Feedback**  
   - Property manager verifies completion.  
   - Tenant is notified and may provide feedback.  

---

## 📊 Business Rules

### Authorization and Access Control
- **Tenant Property Access**: A tenant can only submit requests for the property they occupy (enforced through user claims validation)
- **Property Manager Oversight**: Property managers can only access and manage requests for properties under their supervision (enforced through authorization checks)
- **Unit Assignment Validation**: Tenants can only be assigned to valid units that exist within their designated property

### Request Lifecycle Management  
- **Worker Assignment Requirement**: A request must be assigned to exactly one worker before work can begin (enforced through state machine transitions)
- **Completion Before Closure**: Requests cannot be closed until marked as completed by the assigned worker (enforced through status transition rules)
- **State Transition Validation**: Request status changes must follow the defined lifecycle: Submitted → Scheduled → Done/Failed → Closed

### Data Integrity Rules
- **Contact Information Validation**: All contact information must include valid email addresses and properly formatted names
- **Property Code Uniqueness**: Each property must have a unique identification code within the system
- **Unit Number Consistency**: Unit numbers must exist within the property's defined unit list

---

## 🎯 Domain Goals

- Ensure timely and transparent handling of repair requests.  
- Provide accountability for tenants, managers, and workers.  
- Maintain accurate records of property maintenance history.  
- Support prioritization of urgent issues.  

---

## 📖 Ubiquitous Language

### Core Terms
- **Tenant Request**: The primary entity representing a repair or maintenance issue reported by a tenant (consistent throughout domain and application)
- **Property Manager**: Also referred to as "Superintendent" in the system - the person responsible for overseeing property operations
- **Worker Assignment**: The process and entity linking a repair request to a specific worker for execution
- **Request Status**: The current state of a tenant request in its lifecycle (Submitted, Scheduled, Done, Failed, Closed)

### Status Definitions
- **Submitted**: Request has been created and is awaiting review by property management
- **Scheduled**: Request has been assigned to a worker with a planned service date
- **Done**: Worker has successfully completed the repair work
- **Failed**: Worker was unable to complete the repair (requires rescheduling)
- **Closed**: Request has been finalized and archived by property management

### Entity Relationships
- **Property-Tenant Relationship**: One property can have multiple tenants, each assigned to specific units
- **Tenant-Request Relationship**: One tenant can submit multiple requests over time
- **Request-Worker Assignment**: Each request can have multiple work orders but only one active assignment at a time

---

## ✅ Summary

The Rental Repairs domain revolves around **managing repair requests** for rental properties. It defines the roles of tenants, managers, and workers, the entities they interact with, and the processes that ensure issues are reported, assigned, and resolved effectively. The domain enforces strict business rules through authorization controls, state machine validation, and data integrity constraints to maintain system reliability and user accountability.