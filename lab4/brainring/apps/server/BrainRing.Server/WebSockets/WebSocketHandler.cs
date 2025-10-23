using System.Net.WebSockets;
using System.Text;

namespace BrainRing.Server.WebSockets
{
    public class WebSocketHandler
    {
        private readonly WebSocketConnectionManager _manager;

        public WebSocketHandler(WebSocketConnectionManager manager)
        {
            _manager = manager;
        }

        public async Task HandleConnectionAsync(WebSocket socket)
        {
            var id = _manager.AddSocket(socket);
            Console.WriteLine($"✅ Подключен новый клиент: {id}");

            var buffer = new byte[1024 * 4];
            var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            while (!result.CloseStatus.HasValue)
            {
                var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                Console.WriteLine($"📩 [{id}]: {message}");

                await BroadcastAsync($"👤 {id}: {message}");
                result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }

            await _manager.RemoveSocketAsync(id);
            Console.WriteLine($"❌ Клиент отключён: {id}");
        }

        private async Task BroadcastAsync(string message)
        {
            var data = Encoding.UTF8.GetBytes(message);

            foreach (var socketPair in _manager.GetAll())
            {
                var socket = socketPair.Value;
                if (socket.State == WebSocketState.Open)
                {
                    await socket.SendAsync(
                        new ArraySegment<byte>(data),
                        WebSocketMessageType.Text,
                        true,
                        CancellationToken.None
                    );
                }
            }
        }
    }

}
