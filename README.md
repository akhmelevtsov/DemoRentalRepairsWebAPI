# DemoRentalRepairs

## Description

The repo was started to create a Domain Driven Design example for a simple business workflow and to build it   with ASP .NET Core Web API and Clean Architecture (Onion Architecture). Initially the core service used in-memory repository, then few more were added:  SQL Server and Mongo DB locally and Azure SQL database, Azure Storage Table and Cosmos DB.  

To demonstrate how Clean Architecture helps extend the application, ASP.NET Core MVC project that reuses core services was added. As the domain model requires user roles, the security concern was addressed in the core service layer and ASP.NET Core Identity was added with local SQL server database .

The next task was to publish the application to Azure. The MVC App migrated to Azure App Service and the two SQL Server databases to Azure SQL Database.  To demonstrate the scaling capabilities of the cloud environment, the Notification service was turned into a microservice on Azure with Azure Functions App, Storage Queue and SendGrid. In the process, just two new implementations of the Notification and Email Client service were added with no changes to the core service behaviour.  

Initial migration to Azure revealed some drawbacks - the pricing model for SQL database is not suitable to run demo app even at the lowest pricing tear. So, a new database repository for Azure Storage Tables was created to replace the Azure SQL Database repository, and ASP.NET Core Identity was replaced with Azure AD B2C. The ability to dynamically register user claims was achieved with help of MS Graph API

The development was supported with automated testing, otherwise it would be much more difficult to apply all those modifications. The first unit test was written to simulate 'happy path' - a use case that covers all interactions between domain entities to complete the workflow. Later this test was lifted to integration tests. Few integration tests were also created for email messaging with help of Slurp remote API service and for Azure functions with local Azure Storage emulator

Fig 1. Project dependency diagram

![alt text](https://github.com/akhmelevtsov/DemoRentalRepairsWebAPI/blob/master/Dependencies%20Graph.png?raw=true)

## Domain Model

The domain model contains DDD classes, domain-level enumerations and validations built with the Fluent Validation library.
The domain logic supports a simple workflow scenario:

1 Property manager (superintendent) registers a rental property 
2 Tenant registers himself with the rental property
3.Tenant submits a service request 
4.Superintendent either declines the request or assignes a service worker and schedules the service date
5.Worker either completes the request or rejects the work order
6. If the request is rejected, superintendent re-schedules it 
7. If the request is done, superintendent closes it

The participants are notified by emails on any service request status change

Fig 2. Use Case Diagram


<img src="https://github.com/akhmelevtsov/DemoRentalRepairsWebAPI/blob/master/Use%20Case%20Diagram.png" width=80%>

Fig 3. Service Request State Diagram
<img src="https://github.com/akhmelevtsov/DemoRentalRepairsWebAPI/blob/master/Service%20Request%20State%20Diagram.png" width=80%>

## Core Services

Provide application services abstracted from a specific service implementation: 

 - repository service to store service request status and participants' data
 - notification service to notify participants on every service request status change
 - user identity service to abstract working with identity/authorization

## Infrastructure 

Provides implementations for core interfaces.

Repository services:
- In memory (POCO classes)
- SQL server with EF core 
- Mongo DB 
- Cosmos DB with Mongo DB API
- Azure Table Storage with Table API

Identity services:

- Asp.Net Core Identity 
- Azure AD B2C 

Notification Services:

- Dummy service for turning notifications off
- Slurper Email service for debugging emails
- Azure SendGrid 



## ASP.NET Core MVC application

Simple client that allows users to register, login and interact with core services through the UI. 
Current version is here: https://demorentalrepairswebmvc.azurewebsites.net/

## ASP.NET Core Web API

Utilizes the same core services for mobile or any other client HTTP calls

## Azure Functions

Provide micro-services for building and sending the workflow emails 




