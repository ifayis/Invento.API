# API Reference

## Overview

Invento exposes a RESTful API built with ASP.NET Core Web API.

The API follows common REST conventions and uses:

- JSON Request/Response
- JWT Authentication
- RESTful Resource Naming
- Pagination
- Filtering
- Searching
- Standardized API Responses

Base URL

```
https://localhost:{port}/api/v1
```

---

# Authentication

Most endpoints require a valid JWT Access Token.

Example:

```
Authorization: Bearer {access_token}
```

Public endpoints are marked below.

---

# Standard Response Format

Successful response

```json
{
  "success": true,
  "message": "Success",
  "data": {}
}
```

Validation or business error

```json
{
  "success": false,
  "message": "Validation failed",
  "errors": [
    "Product not found."
  ]
}
```

---

# Authentication APIs

| Method | Endpoint | Auth Required | Description |
|----------|------------------------|--------------|-------------------------------|
| POST | `/auth/register` | No | Register a new tenant and admin user |
| POST | `/auth/login` | No | Authenticate user |
| POST | `/auth/refresh-token` | No | Generate new access token |
| POST | `/auth/logout` | Yes | Logout current session |
| POST | `/auth/forgot-password` | No | Request password reset |
| POST | `/auth/reset-password` | No | Reset password |
| POST | `/auth/change-password` | Yes | Change current password |

---

# User APIs

| Method | Endpoint |
|----------|----------------------|
| GET | `/users` |
| GET | `/users/{id}` |
| POST | `/users` |
| PUT | `/users/{id}` |
| DELETE | `/users/{id}` |
| POST | `/users/{id}/restore` |

---

# Category APIs

| Method | Endpoint |
|----------|--------------------------|
| GET | `/categories` |
| GET | `/categories/{id}` |
| POST | `/categories` |
| PUT | `/categories/{id}` |
| DELETE | `/categories/{id}` |
| POST | `/categories/{id}/restore` |

---

# Product APIs

| Method | Endpoint |
|----------|-------------------------|
| GET | `/products` |
| GET | `/products/{id}` |
| POST | `/products` |
| PUT | `/products/{id}` |
| DELETE | `/products/{id}` |
| POST | `/products/{id}/restore` |

---

# Supplier APIs

| Method | Endpoint |
|----------|-------------------------|
| GET | `/suppliers` |
| GET | `/suppliers/{id}` |
| POST | `/suppliers` |
| PUT | `/suppliers/{id}` |
| DELETE | `/suppliers/{id}` |
| POST | `/suppliers/{id}/restore` |

---

# Customer APIs

| Method | Endpoint |
|----------|-------------------------|
| GET | `/customers` |
| GET | `/customers/{id}` |
| POST | `/customers` |
| PUT | `/customers/{id}` |
| DELETE | `/customers/{id}` |
| POST | `/customers/{id}/restore` |

---

# Purchase APIs

| Method | Endpoint |
|----------|-------------------------|
| GET | `/purchases` |
| GET | `/purchases/{id}` |
| POST | `/purchases` |
| PUT | `/purchases/{id}` |
| DELETE | `/purchases/{id}` |
| POST | `/purchases/{id}/restore` |

---

# Sales APIs

| Method | Endpoint |
|----------|-------------------------|
| GET | `/sales` |
| GET | `/sales/{id}` |
| POST | `/sales` |
| PUT | `/sales/{id}` |
| DELETE | `/sales/{id}` |
| POST | `/sales/{id}/restore` |

---

# Customer Payment APIs

| Method | Endpoint |
|----------|------------------------------|
| POST | `/customer-payments` |
| GET | `/customer-payments` |
| GET | `/customer-payments/{id}` |

---

# Supplier Payment APIs

| Method | Endpoint |
|----------|------------------------------|
| POST | `/supplier-payments` |
| GET | `/supplier-payments` |
| GET | `/supplier-payments/{id}` |

---

# Cash Transaction APIs

| Method | Endpoint |
|----------|-------------------------------|
| POST | `/balance/credit` |
| POST | `/balance/debit` |
| GET | `/balance/transaction` |
| GET | `/balance/cash-flow` |

---

# Dashboard APIs

| Method | Endpoint |
|----------|----------------------------|
| GET | `/dashboard/overview` |
| GET | `/dashboard/top-products` |
| GET | `/dashboard/top-customers` |

---

# Report APIs

| Method | Endpoint |
|----------|---------------------------|
| GET | `/reports/sales` |
| GET | `/reports/purchases` |
| GET | `/reports/profit` |
| GET | `/reports/inventory` |
| GET | `/reports/cash-flow` |

---

# Inventory APIs

| Method | Endpoint |
|----------|----------------------------------|
| GET | `/inventory/stock-movements` |
| GET | `/inventory/low-stock` |
| GET | `/inventory/critical-stock` |

---

# Business Target APIs

| Method | Endpoint |
|----------|-------------------------|
| GET | `/targets` |
| PUT | `/targets` |
| GET | `/targets/progress` |

---

# Pagination

Endpoints returning collections support pagination.

Example

```
GET /products?pageNumber=1&pageSize=10
```

Response

```json
{
    "items": [],
    "pageNumber": 1,
    "pageSize": 10,
    "totalCount": 100
}
```

---

# Searching

Many endpoints support searching.

Example

```
GET /products?search=Milk
```

---

# Filtering

Several endpoints support filtering.

Examples

```
GET /sales?customerId={id}

GET /purchases?supplierId={id}

GET /inventory?categoryId={id}
```

---

# Date Range Filters

Reports support date filtering.

Example

```
GET /reports/sales?fromDate=2026-01-01&toDate=2026-01-31
```

---

# Status Codes

| Status | Meaning |
|----------|------------------------|
| 200 | Success |
| 201 | Resource Created |
| 400 | Validation Error |
| 401 | Unauthorized |
| 403 | Forbidden |
| 404 | Resource Not Found |
| 409 | Conflict |
| 500 | Internal Server Error |

---

# Authentication Notes

Protected endpoints require:

```
Authorization: Bearer {JWT}
```

If the Access Token expires:

1. Call `/auth/refresh-token`
2. Receive a new Access Token
3. Continue using protected APIs

---

# Security Notes

The API includes:

- JWT Authentication
- Refresh Token Rotation
- Password Hashing (BCrypt)
- Password Reset Token Hashing
- Tenant Isolation
- Role-Based Authorization

---

# Swagger

Interactive API documentation is available after running the application.

```
https://localhost:{port}/swagger
```

Swagger allows developers to:

- Explore endpoints
- Authenticate
- Test requests
- View request/response models
- Validate API behavior