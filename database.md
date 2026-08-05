# Database Design

# Entity Relationship Diagram

![ER Diagram](images/er-diagram.png)

## Overview

Invento uses **Microsoft SQL Server** as its primary relational database.

The database is designed to support a **multi-tenant SaaS architecture**, ensuring that each tenant's business data is securely isolated while sharing the same application and database.

The application combines:

- Entity Framework Core for transactional operations
- Dapper for optimized reporting and read-heavy queries

---

# Database Technology

| Component | Technology |
|-----------|------------|
| Database | SQL Server |
| ORM | Entity Framework Core |
| Micro ORM | Dapper |
| Migrations | EF Core Migrations |

---

# Multi-Tenant Design

Invento follows the **Shared Database, Shared Schema** multi-tenant model.

Every business is represented by a unique Tenant.

Nearly all business entities contain a `TenantId` foreign key.

```
Tenant
   │
   ├───────────────┐
   ▼               ▼
Products       Customers
   │               │
   ▼               ▼
Purchases       Sales
```

Every authenticated request filters data using the current tenant.

---

# Database Entities

## Tenant

Represents a business using the system.

### Purpose

- Business Identity
- Tenant Isolation
- Parent Entity

---

## User

Represents an application user.

### Relationships

- Belongs to one Tenant
- Owns Refresh Tokens
- Owns Password Reset Tokens

### Responsibilities

- Authentication
- Authorization
- Password Management

---

## Tenant Settings

Stores tenant-level configuration.

Examples:

- Monthly Sales Target
- Monthly Profit Target

---

## Category

Groups products into logical categories.

### Relationship

One Category

↓

Many Products

---

## Product

Represents inventory items.

### Key Information

- Product Name
- SKU
- Cost Price
- Selling Price
- Current Stock
- Low Stock Threshold
- Critical Stock Threshold

### Relationships

Belongs to

- Category

Referenced by

- Purchase Items
- Sale Items
- Stock Movements

---

## Supplier

Stores supplier information.

### Used By

- Purchases
- Supplier Payments

---

## Customer

Stores customer information.

### Used By

- Sales
- Customer Payments

---

## Purchase

Represents stock purchased from suppliers.

### Contains

- Purchase Number
- Purchase Date
- Totals
- Discount
- Payment Status

### Relationships

One Purchase

↓

Many Purchase Items

---

## Purchase Item

Represents products purchased within a purchase.

Contains:

- Product
- Quantity
- Unit Cost
- Tax
- Total Price

---

## Sale

Represents customer sales.

### Contains

- Invoice Number
- Sale Date
- Profit
- Discount
- Payment Status

### Relationships

One Sale

↓

Many Sale Items

---

## Sale Item

Represents products sold in a sale.

Contains:

- Product
- Quantity
- Unit Price
- Tax
- Profit

---

## Customer Payment

Tracks payments received from customers.

---

## Supplier Payment

Tracks payments made to suppliers.

---

## Cash Transaction

Maintains business cash flow.

Types include:

- Customer Payment
- Supplier Payment
- Manual Credit
- Manual Debit

---

## Stock Movement

Tracks every stock increase and decrease.

Movement Types:

- Purchase
- Sale
- Adjustment

Provides complete inventory history.

---

## Refresh Token

Stores hashed refresh tokens.

Purpose:

- Secure authentication
- Refresh Token Rotation
- Logout
- Session Management

---

## Password Reset Token

Stores password reset requests.

Features:

- Token Hashing
- Expiration
- One-Time Usage

---

## Audit Log

Stores application audit records.

Tracks:

- User Activity
- Entity Changes
- Important Operations

---

## Document Number Sequence

Generates sequential business document numbers.

Examples:

- Invoice Numbers
- Purchase Numbers

---

# Entity Relationship Overview

```
Tenant
│
├── Users
│    ├── Refresh Tokens
│    └── Password Reset Tokens
│
├── Categories
│      │
│      ▼
│   Products
│      │
│      ├─────────────┐
│      ▼             ▼
│ Purchase Items   Sale Items
│      │             │
│      ▼             ▼
│ Purchases       Sales
│
├── Customers
│      │
│      ├── Sales
│      └── Customer Payments
│
├── Suppliers
│      │
│      ├── Purchases
│      └── Supplier Payments
│
├── Stock Movements
│
├── Cash Transactions
│
├── Audit Logs
│
└── Tenant Settings
```

---

# Soft Delete Strategy

Business entities use soft deletion.

Instead of removing records from the database:

```
IsDeleted = true
DeletedAt = Current UTC Time
DeletedBy = Current User
```

Benefits:

- Data recovery
- Auditability
- Historical reporting

---

# Audit Information

Most business entities inherit audit information.

Typical fields include:

- CreatedAt
- CreatedBy
- UpdatedAt
- UpdatedBy
- DeletedAt
- DeletedBy
- IsDeleted

This supports traceability across the application.

---

# Primary Keys

Every entity uses a **GUID** as its primary key.

Benefits:

- Globally unique identifiers
- Suitable for distributed systems
- Easier data synchronization

---

# Foreign Keys

Relationships are enforced using foreign keys.

Examples:

| Child | Parent |
|--------|--------|
| Product | Category |
| Purchase | Supplier |
| Purchase Item | Purchase |
| Purchase Item | Product |
| Sale | Customer |
| Sale Item | Sale |
| Sale Item | Product |
| Customer Payment | Customer |
| Supplier Payment | Supplier |
| Stock Movement | Product |

---

# Indexing Strategy

Indexes are used to improve query performance.

Examples include:

- TenantId
- SKU
- Email
- Document Numbers
- Token Hashes
- Frequently searched columns

This helps optimize filtering, sorting, and reporting operations.

---

# Data Access Strategy

Invento combines two data access approaches.

## Entity Framework Core

Used for:

- Create
- Update
- Delete
- Complex transactional workflows

Reasons:

- Change Tracking
- Relationship Management
- Transactions
- Migrations

---

## Dapper

Used for:

- Dashboard
- Reports
- Read-heavy queries
- Aggregated data
- Performance-critical endpoints

Reasons:

- Minimal overhead
- Optimized SQL
- Faster read operations

---

# Database Principles

The database design follows these principles:

- Multi-Tenant Data Isolation
- Referential Integrity
- Soft Delete
- Auditability
- Optimized Read Performance
- Transactional Consistency
- Scalable Entity Relationships
- Production-Oriented Structure