using SDLS.Services.ApiExceptions;
using System.Text.Json;

namespace SDLS.API.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ApiException ex)
            {
                await WriteApiExceptionAsync(context, ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                await WriteApiExceptionAsync(context, ApiException.Unauthorized(ex.Message));
            }
            catch (KeyNotFoundException ex)
            {
                await WriteApiExceptionAsync(context, ApiException.NotFound(ex.Message));
            }
            catch (ArgumentException ex)
            {
                await WriteApiExceptionAsync(context, ApiException.BadRequest(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                await WriteApiExceptionAsync(context, ApiException.Conflict(ex.Message));
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";

                var result = JsonSerializer.Serialize(new
                {
                    message = "Internal Server Error",
                    detail = ex.Message
                });

                await context.Response.WriteAsync(result);
            }
        }

        private static async Task WriteApiExceptionAsync(HttpContext context, ApiException ex)
        {
            context.Response.StatusCode = ex.StatusCode;
            context.Response.ContentType = "application/json";

            var result = JsonSerializer.Serialize(new
            {
                message = ex.Message,
                status = ex.StatusCode,
                errors = ex.Errors
            });

            await context.Response.WriteAsync(result);
        }
    }
}
