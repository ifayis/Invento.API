//using Invento.API.Common;
//using System.Net;
//using FluentValidation;
//using System.Text.Json;

//namespace Invento.API.Middleware
//{
//    public class ExceptionMiddleware
//    {
//        private readonly RequestDelegate _next;
//        private readonly ILogger<ExceptionMiddleware> _logger;

//        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
//        {
//            _next = next;
//            _logger = logger;
//        }

//        public async Task Invoke(HttpContext context)
//        {
//            try
//            {
//                await _next(context);
//            }
//            catch (Exception ex)
//            {
//                await HandleException(context, ex);
//            }
//        }

//        private async Task HandleException(HttpContext context, Exception ex)
//        {
//            _logger.LogError(ex, ex.Message);

//            var response = new ErrorResponse();

//            switch (ex)
//            {
//                case ValidationException validationEx:
//                    response.StatusCode = (int)HttpStatusCode.BadRequest;
//                    response.Message = "Validation failed";
//                    response.Errors = validationEx.Errors
//                        .GroupBy(e => e.PropertyName)
//                        .ToDictionary(
//                            g => g.Key,
//                            g => g.Select(e => e.ErrorMessage).ToArray()
//                        );
//                    break;

//                case UnauthorizedAccessException:
//                    response.StatusCode = (int)HttpStatusCode.Unauthorized;
//                    response.Message = ex.Message;
//                    break;

//                case KeyNotFoundException:
//                    response.StatusCode = (int)HttpStatusCode.NotFound;
//                    response.Message = ex.Message;
//                    break;

//                default:
//                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
//                    response.Message = "Something went wrong";
//                    break;
//            }

//            context.Response.ContentType = "application/json";
//            context.Response.StatusCode = response.StatusCode;

//            var json = JsonSerializer.Serialize(response);

//            await context.Response.WriteAsync(json);
//        }
//    }
//}
