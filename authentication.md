# Authentication & Authorization

# Authentication Flow

![Authentication Flow](images/authentication-flow.png)

## Overview

Invento implements a secure authentication and authorization system using **JSON Web Tokens (JWT)** and **Refresh Tokens**. The design focuses on security, scalability, and a seamless user experience while following common practices for ASP.NET Core applications.

The authentication module provides:

- User Login
- JWT Access Tokens
- Refresh Token Rotation
- Secure Logout
- Forgot Password
- Reset Password
- Change Password
- Role-Based Authorization
- Tenant-Aware Authentication

---

# Authentication Workflow

```
                    Login
                      │
                      ▼
          Validate Email & Password
                      │
                      ▼
            Generate Access Token
            Generate Refresh Token
                      │
                      ▼
          Return Tokens to Client
                      │
                      ▼
            Access Protected APIs
                      │
          Access Token Expires
                      │
                      ▼
          Refresh Access Token
                      │
                      ▼
           Continue Using API
```

---

# Access Token

Access Tokens are short-lived JWTs that authenticate API requests.

The token contains claims such as:

- User Id
- Tenant Id
- Email
- Role
- Permissions

Every protected request validates the token before accessing business logic.

---

# Refresh Token

Refresh Tokens are long-lived tokens stored securely in the database.

Each refresh token:

- Belongs to one user
- Has an expiration date
- Can be revoked
- Supports token rotation

The refresh token is never used to access protected APIs directly.

Instead, it is exchanged for a new Access Token when the existing one expires.

---

# Refresh Token Rotation

Invento implements refresh token rotation for improved security.

```
Old Refresh Token
        │
        ▼
Validate Token
        │
        ▼
Revoke Old Token
        │
        ▼
Generate New Refresh Token
        │
        ▼
Generate New Access Token
```

Benefits:

- Prevents reuse of compromised refresh tokens.
- Reduces the risk of replay attacks.
- Ensures only the latest refresh token remains active.

---

# Login Flow

```
User
 │
 ▼
Submit Email & Password
 │
 ▼
Validate Credentials
 │
 ▼
Generate JWT
 │
 ▼
Generate Refresh Token
 │
 ▼
Store Refresh Token Hash
 │
 ▼
Return Tokens
```

Passwords are never stored in plain text.

---

# Logout Flow

When the user logs out:

1. The provided Refresh Token is located.
2. The token is revoked.
3. The revocation timestamp is stored.
4. The client must authenticate again to obtain new tokens.

This prevents future use of the revoked refresh token.

---

# Forgot Password

When a password reset is requested:

1. The email address is validated.
2. Existing unused reset tokens are invalidated.
3. A secure random token is generated.
4. The token is hashed before storage.
5. The raw token is sent to the user's email.
6. The token expires automatically after the configured duration.

This approach prevents token theft through database exposure.

---

# Reset Password

```
Receive Reset Token
        │
        ▼
Hash Token
        │
        ▼
Find Matching Token
        │
        ▼
Check Expiration
        │
        ▼
Update Password Hash
        │
        ▼
Mark Reset Token Used
        │
        ▼
Revoke Active Refresh Tokens
```

After a successful password reset:

- The password is updated.
- The reset token cannot be reused.
- Existing refresh tokens are revoked.
- The user must log in again.

---

# Change Password

Authenticated users can change their password.

Validation includes:

- Current password verification.
- New password must differ from the current password.

After a successful password change:

- Password hash is updated.
- Existing refresh tokens are revoked.
- The user signs in again using the new password.

---

# Password Hashing

Invento uses **BCrypt** to hash passwords before storing them.

The application never stores or logs plain-text passwords.

Benefits include:

- Salted hashes
- Adaptive work factor
- Protection against rainbow table attacks

---

# Authorization

Authorization is handled using JWT claims.

Protected endpoints require a valid Access Token.

Claims include:

- User Id
- Tenant Id
- Email
- Role
- Permissions

The authenticated user's claims are resolved through the Current User service and used throughout the application.

---

# Role-Based Authorization

The system supports role-based access.

Examples:

- Administrator
- Staff

Roles determine which features and endpoints a user can access.

---

# Tenant Isolation

Each authenticated request includes a Tenant Id claim.

Every database operation filters records by Tenant Id to ensure complete isolation between businesses sharing the same application.

```
Tenant A
    │
    ▼
Own Products
Own Customers
Own Sales

Tenant B
    │
    ▼
Own Products
Own Customers
Own Sales
```

A tenant cannot access another tenant's data.

---

# Token Storage

Access Tokens are intended for authenticated API requests.

Refresh Tokens are stored securely in the database as hashes rather than plain text.

This reduces the impact of database compromise because the original refresh token value cannot be recovered.

---

# Security Considerations

The authentication system includes multiple security measures:

- BCrypt password hashing
- JWT Access Tokens
- Refresh Token Rotation
- Refresh Token Revocation
- Password Reset Token Hashing
- Password Reset Token Expiration
- Password Reset Token One-Time Usage
- Refresh Token Revocation after Password Reset
- Refresh Token Revocation after Password Change
- Role-Based Authorization
- Tenant-Based Data Isolation

Together, these mechanisms provide a secure authentication and authorization foundation suitable for a production-oriented SaaS application.