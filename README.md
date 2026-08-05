# Invento

> **A Production-Oriented Multi-Tenant Business Management SaaS built with ASP.NET Core Web API**

![.NET](https://img.shields.io/badge/.NET-8-blueviolet)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-Web_API-blue)
![SQL Server](https://img.shields.io/badge/SQL_Server-Database-red)
![Redis](https://img.shields.io/badge/Redis-Caching-red)
![Hangfire](https://img.shields.io/badge/Hangfire-Background_Jobs-green)
![License](https://img.shields.io/badge/License-MIT-blue)

---

## Overview

Invento is a production-oriented **Multi-Tenant Business Management SaaS** designed to help small and medium-sized businesses manage inventory, purchases, sales, customers, suppliers, receivables, payables, cash flow, and business reporting from a single platform.

The application follows **Clean Architecture** with **CQRS and MediatR**, combining **Entity Framework Core** for transactional operations and **Dapper** for high-performance queries.

It is designed with scalability, maintainability, security, and performance as core principles.

---

# Features

### Authentication

- JWT Authentication
- Refresh Token Rotation
- Logout
- Forgot Password
- Reset Password
- Change Password
- BCrypt Password Hashing

---

### Multi-Tenant SaaS

- Tenant-based data isolation
- Tenant-aware request processing
- Shared application, isolated business data

---

### Inventory Management

- Categories
- Products
- Suppliers
- Customers
- Automatic Stock Updates
- Stock Movement History
- Low Stock Monitoring
- Critical Stock Monitoring

---

### Purchase Management

- Create Purchase
- Update Purchase
- Delete Purchase
- Restore Purchase
- Purchase History

---

### Sales Management

- Create Sale
- Update Sale
- Delete Sale
- Restore Sale
- Sales History

---

### Financial Management

- Customer Payments
- Supplier Payments
- Cash Transactions
- Receivables
- Payables

---

### Dashboard & Reports

- Business Dashboard
- Sales Reports
- Purchase Reports
- Inventory Reports
- Cash Flow
- Profit Analysis

---

### Performance

- Dapper
- Redis Distributed Cache
- Cache Versioning
- Cache Invalidation
- SQL Indexing
- Pagination
- Filtering

---

### Background Processing

- Hangfire Background Jobs
- Email Notifications

---

# Technology Stack

| Category | Technology |
|----------|------------|
| Backend | ASP.NET Core Web API (.NET 8) |
| Language | C# |
| Architecture | Clean Architecture |
| Pattern | CQRS + MediatR |
| Database | SQL Server |
| ORM | Entity Framework Core |
| Micro ORM | Dapper |
| Authentication | JWT + Refresh Tokens |
| Caching | Redis |
| Background Jobs | Hangfire |
| Logging | Serilog |
| API Documentation | Swagger |

---

# Solution Structure

```text
Invento.API
Invento.Application
Invento.Domain
Invento.Infrastructure
Invento.Persistence
Invento.Shared
```

---

# Architecture

The project follows **Clean Architecture**, ensuring clear separation of concerns.

```text
                Client
                   ?
                   ?
          ASP.NET Core API
                   ?
                   ?
         Application Layer
      (CQRS + MediatR)
                   ?
                   ?
            Domain Layer
                   ?
          ???????????????????
          ?                 ?
 Persistence          Infrastructure
(EF Core/Dapper)     JWT/Redis/Hangfire
          ?
          ?
      SQL Server
```

---

# Core Modules

- Authentication
- Users
- Categories
- Products
- Customers
- Suppliers
- Purchases
- Sales
- Customer Payments
- Supplier Payments
- Cash Transactions
- Dashboard
- Reports
- Stock Movements
- Business Targets

---

# Documentation

Detailed project documentation is available in the **docs** folder.

| Document | Description |
|----------|-------------|
| architecture.md | System architecture |
| authentication.md | Authentication workflow |
| api.md | API reference |
| database.md | Database design |
| caching.md | Redis caching |
| reports.md | Reporting module |
| security.md | Security implementation |
| deployment.md | Deployment guide |

---

# Getting Started

Clone the repository

```bash
git clone https://github.com/yourusername/Invento.git
```

Restore packages

```bash
dotnet restore
```

Apply migrations

```bash
dotnet ef database update
```

Run the application

```bash
dotnet run
```

Open Swagger

```
https://localhost:{port}/swagger
```

---

# Configuration

Configure the following before running the project:

- SQL Server
- JWT Secret
- SMTP
- Redis
- Hangfire

---

# Screenshots

The following screenshots will be added:

- Login
- Dashboard
- Swagger
- Products
- Sales
- Purchases
- Reports
- Hangfire Dashboard

---

# Future Improvements

- Barcode Support
- Excel Export
- PDF Reports
- Mobile Application
- Multi-Warehouse Support
- Notifications
- Analytics Dashboard

---

# License

This project is licensed under the MIT License.