# DemoRentalRepairsWebAPI

The goal of this project was to build a simple workflow-based application with Domain Driven Design, Clean Architecture (Onion Architecture) and .NET Core technologies as well as  to demonstrate how the core components can be re-used in various configurations 

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

Simple client that allows users to register, login and interact with core services through the UI

## ASP.NET Core Web API

Utilizes the same core services for mobile or any other client HTTP calls

## Azure Functions

Provide micro-services for building and sending the workflow emails 




