using System.Net;
using System.Text.Json;
using static WarehouseManagement.Domain.Common.DomainException;

namespace WarehouseManagement.API.MiddleWares
{
    public class GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        private readonly RequestDelegate _next = next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger = logger;

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            _logger.LogError(ex, "An error occurred while processing request.");

            var response = context.Response;
            response.ContentType = "application/json";

            var statusCodes = ex switch
            {
                KeyNotFoundException => (int)HttpStatusCode.NotFound,          // 404
                UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized, // 401
                InvalidOperationException => (int)HttpStatusCode.BadRequest,     // 400
                ArgumentException => (int)HttpStatusCode.BadRequest,             // 400
                ConflictException => (int)HttpStatusCode.Conflict,        // 409
                _ => (int)HttpStatusCode.InternalServerError                     // 500
            };

            response.StatusCode = statusCodes;

            var result = JsonSerializer.Serialize(new
            {
                statusCode = statusCodes,
                message = ex.Message,
                timestamp = DateTime.UtcNow,
                path = context.Request.Path
            });

            await response.WriteAsync(result);
        }
    }
}
