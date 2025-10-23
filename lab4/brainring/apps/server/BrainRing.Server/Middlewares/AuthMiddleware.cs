using BrainRing.Application.Interfaces.Services;
using BrainRing.Application.Params.User;

namespace BrainRing.Server.Middlewares
{
    public class AuthMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IUserService userService)
        {
            if (context.Request.Method == HttpMethods.Options)
            {
                await _next(context);
                return;
            }

            var path = context.Request.Path.Value;
            if (path != null && (path.Contains("/register") || path.Contains("/login")))
            {
                await _next(context);
                return;
            }

            if (context.Request.Cookies.TryGetValue("userId", out var userIdStr) &&
                Guid.TryParse(userIdStr, out var userId))
            {
                var user = await userService.GetUserByIdAsync(new GetUserByIdParams { Id = userId });
                if (user != null)
                {
                    context.Items["User"] = user;
                    await _next(context);
                    return;
                }
            }

            // Возвращаем 401 и явно добавляем CORS заголовки (страховка)
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.Headers["Access-Control-Allow-Origin"] = "http://localhost:3000";
            context.Response.Headers["Access-Control-Allow-Credentials"] = "true";
            // по необходимости: Access-Control-Allow-Headers/Methods
            await context.Response.WriteAsync("Unauthorized");
        }
    }
}
