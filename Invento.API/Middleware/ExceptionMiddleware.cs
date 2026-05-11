using System.Net;
using System.Text.Json;
using FluentValidation;
using Invento.Application.Common.Models;

namespace Invento.API.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(
        RequestDelegate next,
        ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

            var response = ApiResponse<object>.FailureResponse(
                "Validation failed",
                ex.Errors.Select(x => x.ErrorMessage).ToList());

            await context.Response.WriteAsJsonAsync(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;

            var response = ApiResponse<object>.FailureResponse(
                ex.Message);

            await context.Response.WriteAsJsonAsync(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");

            context.Response.StatusCode =
                (int)HttpStatusCode.InternalServerError;

            var response = ApiResponse<object>.FailureResponse(
                "Internal server error");

            await context.Response.WriteAsJsonAsync(response);
        }
    }
}