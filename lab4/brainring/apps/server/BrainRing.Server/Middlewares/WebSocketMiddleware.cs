using BrainRing.Server.WebSockets;

namespace BrainRing.Server.Middlewares
{
    public class WebSocketMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly WebSocketHandler _handler;

        public WebSocketMiddleware(RequestDelegate next, WebSocketHandler handler)
        {
            _next = next;
            _handler = handler;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path == "/ws")
            {
                if (!context.WebSockets.IsWebSocketRequest)
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    return;
                }

                if (!context.Request.Cookies.TryGetValue("userId", out var userIdStr) ||
                    !Guid.TryParse(userIdStr, out var userId))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return;
                }

                Guid? sessionId = null;
                if (context.Request.Query.TryGetValue("sessionId", out var sessionIdStr) &&
                    Guid.TryParse(sessionIdStr, out var parsedSessionId))
                {
                    sessionId = parsedSessionId;
                }

                bool isHost = false;
                if (context.Request.Query.TryGetValue("isHost", out var isHostStr)) {
                    _ = bool.TryParse(isHostStr, out isHost);
                }

                var socket = await context.WebSockets.AcceptWebSocketAsync();
                await _handler.HandleConnectionAsync(socket, userId, sessionId, isHost);
            }
            else
            {
                await _next(context);
            }
        }
    }
}
