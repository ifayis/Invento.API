# Architecture

# System Architecture

![System Architecture](images/architecture.png)

# Request Flow

![Request Flow](images/request-flow.png)

# Solution Structure

![Solution Structure](images/folder-structure.png)

## Overview

Invento follows **Clean Architecture** to create a scalable, maintainable, and testable backend application. The solution separates business rules from infrastructure concerns, allowing each layer to evolve independently.

The application also implements **CQRS (Command Query Responsibility Segregation)** using **MediatR**, enabling a clear separation between write operations and read operations while improving maintainability.

The architecture is designed around the following principles:

- Separation of Concerns
- Dependency Inversion
- Single Responsibility Principle
- Domain-Centric Design
- Scalability
- Maintainability
- Testability

---

# High-Level Architecture

```
                   Client Applications
        (React / Mobile / Swagger / Postman)
                         │
                         ▼
                ASP.NET Core Web API
                         │
                Controllers / Middleware
                         │
                         ▼
                Application Layer
         (CQRS, MediatR, Validation)
                         │
         ┌───────────────┴───────────────┐
         ▼                               ▼
      Commands                        Queries
         │                               │
         ▼                               ▼
     Business Logic               Read Operations
         │                               │
         └───────────────┬───────────────┘
                         ▼
                    Domain Layer
          (Entities, Enums, Interfaces)
                         │
                         ▼
        Infrastructure & Persistence
   JWT • Redis • Hangfire • Serilog • SMTP
             EF Core • Dapper
                         │
                         ▼
                    SQL Server
```

---

# Solution Structure

```
Invento.API
Invento.Application
Invento.Domain
Invento.Infrastructure
Invento.Persistence
Invento.Shared
```

Each project has a single responsibility.

---

# Invento.API

The API project acts as the application's entry point.

Responsibilities include:

- Controllers
- Dependency Injection
- Middleware
- Swagger Configuration
- Authentication Configuration
- Authorization Policies
- Global Exception Handling
- Application Startup

The API layer contains no business logic.

---

# Invento.Application

This layer contains all application use cases.

Responsibilities include:

- Commands
- Queries
- Handlers
- DTOs
- Validators
- CQRS
- MediatR
- Mapping Extensions
- Cache Keys
- Business Rules

Every feature is organized independently.

Example:

```
Products
│
├── Commands
├── Queries
├── DTOs
├── Extensions
└── Validators
```

---

# Invento.Domain

The Domain layer represents the business itself.

Responsibilities:

- Entities
- Enums
- Domain Constants
- Interfaces
- Business Models

This layer has no dependency on EF Core, ASP.NET Core, or any external framework.

It contains only business concepts.

---

# Invento.Persistence

Responsible for data access.

Responsibilities:

- DbContext
- Entity Configurations
- Migrations
- SQL Server
- Repository Implementations
- Dapper Connection Factory

Entity Framework Core is used for transactional operations.

Dapper is used for reporting and optimized read queries.

---

# Invento.Infrastructure

Provides implementations for external services.

Responsibilities:

- JWT Token Generation
- Authentication
- Redis Cache
- Serilog
- Email Service
- Hangfire
- Current User Service
- Current Tenant Service

Infrastructure depends on the Application layer but never the other way around.

---

# Invento.Shared

Contains reusable components shared across multiple projects.

Examples:

- BaseEntity
- AuditableEntity
- Pagination Models
- Common Utilities
- Shared Constants

---

# Clean Architecture

Dependencies always point inward.

```
API
 │
 ▼
Application
 │
 ▼
Domain

Infrastructure ───────┐
Persistence ──────────┘
        │
        ▼
Application
```

The Domain layer remains independent of frameworks and infrastructure.

---

# CQRS

Invento separates read operations from write operations.

### Commands

Commands modify data.

Examples:

- Create Product
- Update Product
- Delete Product
- Create Purchase
- Create Sale

Commands return only the data required after the operation.

---

### Queries

Queries only retrieve data.

Examples:

- Get Products
- Dashboard
- Reports
- Low Stock Products
- Top Customers

Queries never modify data.

---

# Why MediatR?

MediatR provides an in-process mediator that decouples controllers from business logic.

Instead of calling services directly:

```
Controller
    │
    ▼
Handler
```

The controller sends a request to MediatR, which locates the appropriate handler.

Benefits:

- Loose coupling
- Easier testing
- Better organization
- Independent features
- Cleaner controllers

---

# Why Entity Framework Core?

Entity Framework Core is used where change tracking and transactional consistency are important.

Examples:

- Creating Sales
- Updating Purchases
- Authentication
- Password Reset
- Stock Updates

Benefits:

- Change Tracking
- Migrations
- Transactions
- Relationship Management

---

# Why Dapper?

Dapper is used for read-heavy operations.

Examples:

- Reports
- Dashboard
- Top Customers
- Sales Summary
- Purchase Summary

Benefits:

- Faster execution
- Lightweight mapping
- Optimized SQL
- Better reporting performance

---

# Multi-Tenant Architecture

Invento is designed as a shared-database, shared-application multi-tenant SaaS.

Every business has its own Tenant.

Every request is filtered using the authenticated TenantId.

```
Tenant A
   │
   ▼
Products
Sales
Customers

Tenant B
   │
   ▼
Products
Sales
Customers
```

No tenant can access another tenant's data.

---

# Authentication Flow

The application uses JWT Authentication with Refresh Tokens.

```
Login
   │
   ▼
Generate Access Token
Generate Refresh Token
   │
   ▼
Protected APIs
   │
   ▼
Refresh Token Rotation
   │
   ▼
Logout
```

Passwords are stored using BCrypt hashing.

---

# Caching

Redis Distributed Cache improves application performance.

Cache implementation includes:

- Cache Groups
- Cache Keys
- Cache Versioning
- Automatic Invalidation

Whenever data changes, related cache groups are invalidated automatically.

---

# Background Jobs

Hangfire is responsible for background processing.

Current responsibilities include:

- Email Sending

The architecture allows future scheduled jobs without changing the application structure.

---

# Logging

Serilog provides structured logging.

Application logs include:

- Requests
- Exceptions
- Background Jobs
- Application Events

This simplifies debugging and monitoring.

---

# Error Handling

The application uses centralized exception handling.

Benefits:

- Consistent API responses
- Cleaner handlers
- Reduced duplicate code
- Easier maintenance

---

# Design Goals

The primary goals of the architecture are:

- Scalability
- Maintainability
- Performance
- Security
- Readability
- Testability
- Separation of Concerns

These principles make Invento suitable as a foundation for real-world business management SaaS applications.