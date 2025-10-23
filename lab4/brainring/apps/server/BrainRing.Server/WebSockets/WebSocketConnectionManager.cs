using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace BrainRing.Server.WebSockets
{
    public class WebSocketConnectionManager
    {
        private readonly ConcurrentDictionary<string, (WebSocket Socket, Guid UserId, Guid? SessionId)> _sockets = new();

        public string AddSocket(WebSocket socket, Guid userId, Guid? sessionId)
        {
            var id = Guid.NewGuid().ToString();
            _sockets.TryAdd(id, (socket, userId, sessionId));
            return id;
        }

        public IEnumerable<KeyValuePair<string, WebSocket>> GetAllBySession(Guid sessionId)
        {
            return _sockets
                .Where(p => p.Value.SessionId == sessionId)
                .ToDictionary(k => k.Key, v => v.Value.Socket);
        }

        public async Task RemoveSocketAsync(string id)
        {
            if (_sockets.TryRemove(id, out var socketInfo))
            {
                await socketInfo.Socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by server", CancellationToken.None);
                socketInfo.Socket.Dispose();
            }
        }
    }
}
