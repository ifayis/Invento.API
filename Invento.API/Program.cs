using Hangfire;
using Invento.API.Extensions;
using Invento.API.Hangfire;
using Invento.API.Health;
using Invento.API.Middleware;
using Invento.API.Swagger;
using Invento.Application.Common;
using Invento.Application.Common.Jobs;
using Invento.Application.Common.Services;
using Invento.Application.Features.Auth.Commands;
using Invento.Application.Interfaces;
using Invento.Infrastructure.Auth;
using Invento.Infrastructure.Extensions;
using Invento.Persistence.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.AddServerHeader = false;
});

builder.Services.Configure<ForwardedHeadersOptions>(
    options =>
    {
        options.ForwardedHeaders =
            ForwardedHeaders.XForwardedFor |
            ForwardedHeaders.XForwardedProto;
    });

builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .Enrich.WithProperty(
            "Application",
            "Invento.API");
});

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode =
        StatusCodes.Status429TooManyRequests;

    options.OnRejected = async (
        context,
        cancellationToken) =>
    {
        context.HttpContext.Response.ContentType =
            "application/json";

        await context.HttpContext.Response.WriteAsJsonAsync(
            new
            {
                Message =
                    "Too many requests. Please try again later."
            },
            cancellationToken:
                cancellationToken);
    };

    options.AddPolicy(
        "AuthPolicy",
        httpContext =>
        {
            var remoteIpAddress =
                httpContext.Connection
                    .RemoteIpAddress;

            var partitionKey =
                remoteIpAddress is null
                    ? "unknown"
                    : remoteIpAddress
                        .MapToIPv4()
                        .ToString();

            return RateLimitPartition
                .GetFixedWindowLimiter(
                    partitionKey:
                        partitionKey,

                    factory: _ =>
                        new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 5,

                            Window =
                                TimeSpan.FromMinutes(1),

                            QueueLimit = 0,

                            QueueProcessingOrder =
                                QueueProcessingOrder
                                    .OldestFirst,

                            AutoReplenishment = true
                        });
        });
});

builder.Services.AddControllers();

var allowedOrigins =
    builder.Configuration
        .GetSection("Cors:AllowedOrigins")
        .Get<string[]>()
    ?? Array.Empty<string>();

if (builder.Environment.IsProduction() &&
    allowedOrigins.Length == 0)
{
    throw new InvalidOperationException(
        "Cors:AllowedOrigins must be configured " +
        "for Production.");
}

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "FrontendPolicy",
        policy =>
        {
            policy
                .WithOrigins(allowedOrigins)
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddResponseCompressionServices();

builder.Services.AddApiVersioningServices();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();

builder.Services.AddApplicationServices();

var defaultConnectionString =
    builder.Configuration.GetConnectionString(
        "DefaultConnection");

if (string.IsNullOrWhiteSpace(
    defaultConnectionString))
{
    throw new InvalidOperationException(
        "ConnectionStrings:DefaultConnection " +
        "is not configured.");
}

var redisConnectionString =
    builder.Configuration[
        "Redis:ConnectionString"];

if (string.IsNullOrWhiteSpace(
    redisConnectionString))
{
    throw new InvalidOperationException(
        "Redis:ConnectionString " +
        "is not configured.");
}

if (builder.Environment.IsProduction())
{
    if (defaultConnectionString.Contains(
        "localhost",
        StringComparison.OrdinalIgnoreCase))
    {
        throw new InvalidOperationException(
            "Production SQL Server configuration " +
            "must not use localhost.");
    }

    if (defaultConnectionString.Contains(
        @"\SQLEXPRESS",
        StringComparison.OrdinalIgnoreCase))
    {
        throw new InvalidOperationException(
            "Production SQL Server configuration " +
            "must not use SQL Express.");
    }

    if (redisConnectionString.Contains(
        "localhost",
        StringComparison.OrdinalIgnoreCase))
    {
        throw new InvalidOperationException(
            "Production Redis configuration " +
            "must not use localhost.");
    }
}

builder.Services.AddPersistenceServices(
    builder.Configuration);

builder.Services.AddInfrastructureServices(
    builder.Configuration);
builder.Services.AddScoped<StockMovementService>();

builder.Services.AddScoped<CashTransactionService>();

builder.Services.AddPermissionPolicies();

builder.Services.AddHealthCheckServices(builder.Configuration);

builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings"));

var jwtSettings = builder.Configuration
    .GetSection("JwtSettings")
    .Get<JwtSettings>()
    ?? throw new InvalidOperationException(
        "JwtSettings configuration is missing.");

if (string.IsNullOrWhiteSpace(
    jwtSettings.SecretKey))
{
    throw new InvalidOperationException(
        "JwtSettings:SecretKey is not configured.");
}

if (Encoding.UTF8.GetByteCount(
        jwtSettings.SecretKey) < 32)
{
    throw new InvalidOperationException(
        "JwtSettings:SecretKey must be at least " +
        "32 bytes long.");
}

if (string.IsNullOrWhiteSpace(
    jwtSettings.Issuer))
{
    throw new InvalidOperationException(
        "JwtSettings:Issuer is not configured.");
}

if (string.IsNullOrWhiteSpace(
    jwtSettings.Audience))
{
    throw new InvalidOperationException(
        "JwtSettings:Audience is not configured.");
}

if (jwtSettings.AccessTokenExpirationMinutes <= 0)
{
    throw new InvalidOperationException(
        "JwtSettings:AccessTokenExpirationMinutes " +
        "must be greater than zero.");
}

if (jwtSettings.RefreshTokenExpirationDays <= 0)
{
    throw new InvalidOperationException(
        "JwtSettings:RefreshTokenExpirationDays " +
        "must be greater than zero.");
}
builder.Services
    .AddAuthentication(
        JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,

                ValidateAudience = true,

                ValidateLifetime = true,

                ValidateIssuerSigningKey = true,

                ValidIssuer =
                    jwtSettings.Issuer,

                ValidAudience =
                    jwtSettings.Audience,

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            jwtSettings.SecretKey)),

                NameClaimType = "Name",

                RoleClaimType = "Role",

                ClockSkew =
                    TimeSpan.FromSeconds(30)
            };
    });

var emailSettings = builder.Configuration
    .GetSection("EmailSettings")
    .Get<EmailSettings>()
    ?? throw new InvalidOperationException(
        "EmailSettings configuration is missing.");

if (string.IsNullOrWhiteSpace(
    emailSettings.Host))
{
    throw new InvalidOperationException(
        "EmailSettings:Host is not configured.");
}

if (emailSettings.Port <= 0 ||
    emailSettings.Port > 65535)
{
    throw new InvalidOperationException(
        "EmailSettings:Port must be between 1 and 65535.");
}

if (string.IsNullOrWhiteSpace(
    emailSettings.Username))
{
    throw new InvalidOperationException(
        "EmailSettings:Username is not configured.");
}

if (string.IsNullOrWhiteSpace(
    emailSettings.Password))
{
    throw new InvalidOperationException(
        "EmailSettings:Password is not configured.");
}

if (string.IsNullOrWhiteSpace(
    emailSettings.FromEmail))
{
    throw new InvalidOperationException(
        "EmailSettings:FromEmail is not configured.");
}

if (string.IsNullOrWhiteSpace(
    emailSettings.FromName))
{
    throw new InvalidOperationException(
        "EmailSettings:FromName is not configured.");
}

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI(options =>
    {
        var provider =
            app.Services.GetRequiredService<
                Asp.Versioning.ApiExplorer
                .IApiVersionDescriptionProvider>();

        foreach (var description in
            provider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint(
                $"/swagger/{description.GroupName}/swagger.json",
                $"Invento API {description.GroupName.ToUpperInvariant()}");
        }
    });
}

app.UseForwardedHeaders();

var httpsRedirectionEnabled =
    builder.Configuration.GetValue<bool>(
        "HttpsRedirection:Enabled");

if (httpsRedirectionEnabled)
{
    app.UseHttpsRedirection();
}

app.Use(async (context, next) =>
{
    context.Response.OnStarting(() =>
    {
        var headers =
            context.Response.Headers;

        headers["X-Content-Type-Options"] =
            "nosniff";

        headers["X-Frame-Options"] =
            "DENY";

        headers["Referrer-Policy"] =
            "no-referrer";

        headers["Permissions-Policy"] =
            "camera=(), microphone=(), geolocation=()";

        return Task.CompletedTask;
    });

    await next();
});

app.UseResponseCompression();

app.UseCors("FrontendPolicy");

app.UseRateLimiter();

app.UseAuthentication();

app.UseRequestLoggingMiddleware();

app.UseCustomExceptionMiddleware();

app.UseAuthorization();

app.UseHangfireDashboard(
    "/hangfire",
    new DashboardOptions
    {
        Authorization =
        [
            new HangfireAuthorizationFilter()
        ]
    });

app.MapControllers();

var registerRecurringJobs =
    builder.Configuration.GetValue<bool>(
        "Hangfire:RegisterRecurringJobs");

if (registerRecurringJobs)
{
    using var scope =
        app.Services.CreateScope();

    var recurringJobs =
        scope.ServiceProvider
            .GetRequiredService<IRecurringJobService>();

    RecurringJob.AddOrUpdate(
        "low-stock-check",
        () => recurringJobs.ExecuteLowStockCheck(),
        Cron.Hourly);

    RecurringJob.AddOrUpdate(
        "sales-target-check",
        () => recurringJobs.ExecuteSalesTargetCheck(),
        Cron.Daily);

    RecurringJob.AddOrUpdate(
        "profit-target-check",
        () => recurringJobs.ExecuteProfitTargetCheck(),
        Cron.Daily);

    RecurringJob.AddOrUpdate(
        "receivable-check",
        () => recurringJobs.ExecuteReceivableCheck(),
        Cron.Daily);

    RecurringJob.AddOrUpdate(
        "payable-check",
        () => recurringJobs.ExecutePayableCheck(),
        Cron.Daily);

    RecurringJob.AddOrUpdate(
        "refresh-token-cleanup",
        () => recurringJobs.ExecuteRefreshTokenCleanup(),
        Cron.Daily);
}

app.MapHealthChecks(
    "/health/live",
    new HealthCheckOptions
    {
        Predicate = check =>
            check.Tags.Contains("live"),

        ResponseWriter = async (
            context,
            report) =>
        {
            context.Response.ContentType =
                "application/json";

            await context.Response.WriteAsJsonAsync(
                new
                {
                    Status =
                        report.Status.ToString()
                });
        }
    });

app.MapHealthChecks(
    "/health/ready",
    new HealthCheckOptions
    {
        Predicate = check =>
            check.Tags.Contains("ready"),

        ResponseWriter = async (
            context,
            report) =>
        {
            context.Response.ContentType =
                "application/json";

            await context.Response.WriteAsJsonAsync(
                new
                {
                    Status =
                        report.Status.ToString()
                });
        }
    });

if (app.Environment.IsDevelopment())
{
    app.MapHealthChecks(
        "/health",
        new HealthCheckOptions
        {
            Predicate = _ => true,

            ResponseWriter =
                HealthCheckResponseWriter
                    .WriteResponseAsync
        });
}
try
{
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(
        ex,
        "Invento API terminated unexpectedly.");
}
finally
{
    Log.CloseAndFlush();
}