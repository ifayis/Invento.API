# Deployment Guide

## Overview

This document explains how to configure, run, and deploy the Invento backend application.

Invento is built using:

- ASP.NET Core Web API (.NET 8)
- SQL Server
- Redis
- Hangfire
- Serilog

The application can be executed locally for development or deployed to a cloud hosting platform for production use.

---

# Prerequisites

Install the following software before running the project.

## .NET SDK

Version

```
.NET 8 SDK
```

Verify installation

```bash
dotnet --version
```

---

## SQL Server

Install one of the following:

- SQL Server Express
- SQL Server Developer Edition
- SQL Server Standard

Also install SQL Server Management Studio (SSMS).

---

## Redis

Install Redis locally or use a hosted Redis service.

Example connection

```
localhost:6379
```

---

## Git

Clone the repository using Git.

---

# Clone Repository

```bash
git clone https://github.com/yourusername/Invento.git

cd Invento
```

---

# Restore Dependencies

```bash
dotnet restore
```

---

# Configure Application Settings

Update **appsettings.Development.json** (or use environment variables in production).

Example configuration:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=Invento;Trusted_Connection=True;TrustServerCertificate=True;"
  },

  "Jwt": {
    "Issuer": "Invento",
    "Audience": "InventoUsers",
    "SecretKey": "YOUR_SECRET_KEY",
    "AccessTokenExpirationMinutes": 30,
    "RefreshTokenExpirationDays": 7
  },

  "Redis": {
    "ConnectionString": "localhost:6379"
  },

  "Email": {
    "Host": "smtp.example.com",
    "Port": 587,
    "Username": "user@example.com",
    "Password": "password"
  }
}
```

> **Important:** Never commit production secrets or passwords to Git. Use environment variables or a secure secret store.

---

# Apply Database Migrations

Create or update the database.

```bash
dotnet ef database update
```

---

# Run the Application

```bash
dotnet run
```

The API will start on the configured HTTP/HTTPS ports.

---

# Open Swagger

After the application starts, open:

```
https://localhost:{port}/swagger
```

Swagger provides interactive documentation for all available endpoints.

---

# Hangfire Dashboard

If enabled, the Hangfire Dashboard is available at:

```
https://localhost:{port}/hangfire
```

Use this dashboard to monitor background jobs.

---

# Redis

Redis is used for distributed caching.

Verify Redis is running before starting the application.

Example:

```bash
redis-server
```

---

# Logging

Serilog automatically records:

- Application events
- Errors
- Exceptions
- Request information

Configure log storage according to your deployment environment.

---

# Build for Release

```bash
dotnet publish -c Release
```

The compiled application will be generated inside:

```
bin/Release/net8.0/publish
```

---

# Deployment Options

Invento can be deployed to:

- Microsoft Azure App Service
- Azure Virtual Machine
- IIS
- Render
- Railway
- Docker
- Linux VPS
- Windows Server

The application is independent of a specific hosting provider.

---

# Environment Variables

Recommended production environment variables include:

| Variable | Description |
|----------|-------------|
| ConnectionStrings__DefaultConnection | SQL Server connection string |
| Jwt__SecretKey | JWT signing key |
| Jwt__Issuer | JWT issuer |
| Jwt__Audience | JWT audience |
| Redis__ConnectionString | Redis server |
| Email__Host | SMTP server |
| Email__Username | SMTP username |
| Email__Password | SMTP password |

---

# Production Checklist

Before deploying, verify:

- HTTPS enabled
- Database backups configured
- Strong JWT secret key
- Production SMTP credentials
- Redis configured
- Logging configured
- Swagger disabled or protected in production (optional)
- Hangfire Dashboard secured
- Environment variables configured
- No development secrets committed

---

# Backup Strategy

Recommended backups:

- SQL Server database
- Uploaded files (if applicable)
- Configuration backups
- Log retention policy

---

# Monitoring

Recommended monitoring includes:

- Application availability
- SQL Server health
- Redis availability
- Hangfire job status
- Disk usage
- Memory usage
- CPU utilization
- Error logs

---

# Troubleshooting

## Database Connection Failed

- Verify SQL Server is running.
- Check the connection string.
- Confirm firewall settings.

---

## Redis Connection Failed

- Verify Redis is running.
- Check the Redis connection string.

---

## JWT Authentication Fails

- Verify the secret key.
- Check issuer and audience configuration.
- Confirm token expiration.

---

## Email Sending Failed

- Verify SMTP credentials.
- Check firewall/network access.
- Ensure SMTP server allows the configured account.

---

## Migration Errors

Run:

```bash
dotnet ef database update
```

If necessary, review migration history and database schema before reapplying migrations.

---

# Security Recommendations

For production deployments:

- Enforce HTTPS.
- Store secrets outside source control.
- Rotate JWT secret keys when appropriate.
- Restrict database access.
- Secure Redis instances.
- Protect Hangfire Dashboard.
- Keep dependencies updated.
- Monitor application logs.

---

# Deployment Summary

Deployment consists of:

1. Clone the repository.
2. Restore dependencies.
3. Configure settings.
4. Apply database migrations.
5. Start SQL Server and Redis.
6. Run the application.
7. Verify Swagger.
8. Monitor logs and background jobs.

Following these steps provides a repeatable deployment process for both development and production environments.