using BrainRing.Server.Exceptions;
using System.Net;
using System.Text.Json;

namespace BrainRing.Server.Middleware
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                if (ex is OperationCanceledException) return;

                Console.WriteLine(ex.Message);

                await _handleExceptionAsync(context, ex);
            }
        }

        private async Task _handleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var response = context.Response;

            response.StatusCode = exception switch
            {
                BadRequestException => (int)HttpStatusCode.BadRequest,
                NotFoundException => (int)HttpStatusCode.NotFound,
                ArgumentException => (int)HttpStatusCode.BadRequest,
                InternalServerException => (int)HttpStatusCode.InternalServerError,
                _ => (int)HttpStatusCode.InternalServerError,
            };

            var errorResponse = new
            {
                message = response.StatusCode == (int)HttpStatusCode.InternalServerError 
                    ? "Internal server error" 
                    : exception.Message,
                statusCode = response.StatusCode
            };

            var result = JsonSerializer.Serialize(errorResponse);

            await context.Response.WriteAsync(result);
        }
    }

}
