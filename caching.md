# Caching Strategy

## Overview

Invento uses **Redis Distributed Cache** to improve API response times and reduce unnecessary database queries.

Instead of repeatedly querying SQL Server for frequently requested data, selected query results are cached in Redis.

The application implements a **cache versioning strategy**, allowing cache invalidation without explicitly deleting individual cache entries.

---

# Why Redis?

Redis was selected because it provides:

- In-memory data storage
- Very low latency
- Distributed caching
- Scalability
- High throughput

---

# Cached Modules

Caching is applied to frequently accessed read operations, including:

- Dashboard
- Products
- Categories
- Customers
- Suppliers
- Purchases
- Sales
- Reports
- Inventory
- Business Targets

---

# Cache Architecture

```
Client
   │
   ▼
API Request
   │
   ▼
Redis Cache
   │
 ┌─┴─────────────┐
 │ Cache Exists? │
 └─┬─────────────┘
   │Yes
   ▼
Return Cached Data

No
 │
 ▼
SQL Server
 │
 ▼
Store Result in Redis
 │
 ▼
Return Response
```

---

# Cache Groups

Invento organizes cached data into logical groups.

Examples:

- Products
- Categories
- Customers
- Suppliers
- Purchases
- Sales
- Dashboard
- Reports
- Inventory
- Targets

Grouping simplifies invalidation after data changes.

---

# Cache Keys

Each cache entry has a unique key based on:

- Module
- Tenant
- Query Parameters
- Page Number
- Page Size
- Filters
- Search Terms

This ensures different requests are cached independently.

---

# Cache Versioning

Instead of deleting individual keys, Invento uses cache versioning.

Each cache group maintains a version number.

When data changes:

1. Increment cache version.
2. Future requests generate new cache keys.
3. Old cache entries naturally expire.

Benefits:

- Simplified invalidation
- Better scalability
- Reduced Redis operations

---

# Cache Invalidation

Whenever data changes, related cache groups are invalidated.

Examples:

## Product Updated

Invalidate:

- Products
- Dashboard
- Reports

---

## Purchase Created

Invalidate:

- Purchases
- Products
- Dashboard
- Reports
- Payables
- Balance

---

## Sale Created

Invalidate:

- Sales
- Dashboard
- Reports
- Receivables
- Products
- Balance

---

# Expiration

Cached responses have configurable expiration times.

Typical categories include:

- Short
- Medium
- Long

The duration depends on how frequently the data changes.

---

# Benefits

The caching strategy provides:

- Faster response times
- Lower database load
- Better scalability
- Improved user experience
- Efficient cache invalidation