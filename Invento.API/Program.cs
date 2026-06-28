using Hangfire;
using Invento.API.Extensions;
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
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRateLimiter(options =>
{
    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.ContentType =
            "application/json";

        await context.HttpContext.Response.WriteAsJsonAsync(
            new
            {
                Message =
                    "Too many requests. Please try again later."
            },
            cancellationToken: token);
    };

    options.AddPolicy(
        "AuthPolicy",
        httpContext =>
        {
            return RateLimitPartition.GetFixedWindowLimiter(
                partitionKey:
                    httpContext.Connection.RemoteIpAddress?.ToString()
                    ?? "unknown",

                factory: _ =>
                    new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 5,

                        Window = TimeSpan.FromMinutes(1),

                        QueueLimit = 0,

                        AutoReplenishment = true
                    });
        });
});

builder.Services.AddControllers();

builder.Services.AddHttpContextAccessor();

builder.Services.AddResponseCompressionServices();

builder.Services.AddApiVersioningServices();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();

builder.Services.AddApplicationServices();

builder.Services.AddPersistenceServices(builder.Configuration);

builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddScoped<StockMovementService>();

builder.Services.AddScoped<CashTransactionService>();

builder.Services.AddPermissionPolicies();

builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings"));

var jwtSettings = builder.Configuration
    .GetSection("JwtSettings")
    .Get<JwtSettings>();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,

                ValidateAudience = true,

                ValidateLifetime = true,

                ValidateIssuerSigningKey = true,

                ValidIssuer = jwtSettings!.Issuer,

                ValidAudience = jwtSettings.Audience,

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            jwtSettings.SecretKey)),

                NameClaimType = "Name",

                RoleClaimType = "Role"
            };
    });

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

app.UseHangfireDashboard("/hangfire");

app.UseCustomExceptionMiddleware();

app.UseRequestLoggingMiddleware();

app.UseHttpsRedirection();

app.UseResponseCompression();

app.UseRateLimiter();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
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
}

app.Run();