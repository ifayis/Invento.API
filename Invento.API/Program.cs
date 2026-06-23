using Hangfire;
using Invento.API.Middleware;
using Invento.Application.Common;
using Invento.Application.Common.Jobs;
using Invento.Application.Common.Services;
using Invento.Application.Features.Auth.Commands;
using Invento.Application.Interfaces;
using Invento.Infrastructure.Extensions;
using Invento.Persistence.Data;
using Invento.Persistence.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc(
        "v1",
        new OpenApiInfo
        {
            Title = "Invento API",
            Version = "v1"
        });

    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Enter JWT Bearer token ONLY",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Id = "Bearer",
            Type = ReferenceType.SecurityScheme
        }
    };

    options.AddSecurityDefinition(
        "Bearer", securityScheme
    );

    options.AddSecurityRequirement(
        new OpenApiSecurityRequirement
        {
            {
                securityScheme,
                Array.Empty<string>()
            }
        });
});

builder.Services.AddApplicationServices();
builder.Services.AddPersistenceServices(builder.Configuration);
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddScoped<StockMovementService>();
builder.Services.AddScoped<CashTransactionService>();

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
                            jwtSettings.SecretKey)
                    ),

                NameClaimType = "Name",
                RoleClaimType = "Role"
            };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHangfireDashboard("/hangfire");

app.UseCustomExceptionMiddleware();

app.UseRequestLoggingMiddleware();

app.UseHttpsRedirection();

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