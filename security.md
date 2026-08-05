# Security

## Overview

Security is a core design principle of Invento.

The application protects authentication, authorization, business data, and tenant isolation using multiple security mechanisms.

---

# Authentication

Invento uses:

- JWT Access Tokens
- Refresh Tokens
- BCrypt Password Hashing

Passwords are never stored in plain text.

---

# Password Security

Passwords are hashed using BCrypt before storage.

Benefits:

- Salted Hashes
- Adaptive Work Factor
- Resistance to Rainbow Table Attacks

---

# Refresh Tokens

Refresh Tokens provide secure session renewal.

Features include:

- Token Rotation
- Revocation
- Expiration
- Database Hashing

---

# Password Reset

Password reset includes:

- Cryptographically Secure Tokens
- SHA-256 Token Hashing
- One-Time Usage
- Token Expiration
- Refresh Token Revocation

---

# Change Password

Changing a password automatically:

- Updates the password hash
- Revokes active refresh tokens
- Requires re-authentication

---

# Authorization

Protected endpoints require a valid JWT.

Role-based authorization controls access to administrative operations.

---

# Multi-Tenant Security

Every authenticated request contains a TenantId claim.

Every query filters records using TenantId.

This prevents data leakage between tenants.

---

# Soft Delete

Business records are not permanently removed.

Instead:

- IsDeleted
- DeletedAt
- DeletedBy

This preserves auditability and supports recovery.

---

# Audit Logging

Important operations are recorded through audit logging.

Examples include:

- Business operations
- User activity
- Data modifications

---

# Input Validation

Commands are validated before execution.

Validation prevents:

- Missing required values
- Invalid identifiers
- Invalid business rules

---

# SQL Injection Protection

Entity Framework Core uses parameterized queries.

Dapper queries also use parameterized SQL.

This protects against SQL injection attacks.

---

# API Security

Security measures include:

- HTTPS support
- JWT Authentication
- Role Authorization
- Tenant Isolation
- Password Hashing
- Secure Token Storage

---

# Security Summary

Invento incorporates multiple layers of security:

- JWT Authentication
- Refresh Token Rotation
- BCrypt Password Hashing
- Password Reset Token Hashing
- Tenant Isolation
- Role-Based Authorization
- Parameterized SQL
- Soft Delete
- Audit Logging

These mechanisms provide a strong security foundation for a production-oriented SaaS application.