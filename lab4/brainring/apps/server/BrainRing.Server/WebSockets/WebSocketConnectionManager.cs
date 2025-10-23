using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace BrainRing.Server.WebSockets
{
    public class WebSocketConnectionManager
    {
        private readonly ConcurrentDictionary<string, WebSocket> _sockets = new();

        public string AddSocket(WebSocket socket)
        {
            var id = Guid.NewGuid().ToString();
            _sockets.TryAdd(id, socket);
            return id;
        }

        public WebSocket? GetSocketById(string id)
        {
            _sockets.TryGetValue(id, out var socket);
            return socket;
        }

        public IEnumerable<KeyValuePair<string, WebSocket>> GetAll() => _sockets;

        public async Task RemoveSocketAsync(string id)
        {
            if (_sockets.TryRemove(id, out var socket))
            {
                await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by server", CancellationToken.None);
                socket.Dispose();
            }
        }
    }
}
