using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Text.Json;
using server.dto;

namespace Server
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            int port = 5000;
            if (args.Length >= 2 && args[0] == "--port") int.TryParse(args[1], out port);

            var ipPoint = new IPEndPoint(IPAddress.Any, port);
            using var listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(ipPoint);
            listener.Listen(100);

            Console.WriteLine($"Server listening on port {port}");

            while (true)
            {
                var client = await listener.AcceptAsync();
                _ = Task.Run(async () =>
                {
                    Console.WriteLine($"Client connected: {client.RemoteEndPoint}");
                    var buffer = new byte[10 * 1024 * 1024];
                    try
                    {
                        while (true)
                        {
                            int received = await client.ReceiveAsync(buffer, SocketFlags.None);
                            if (received == 0) break;
                            var json = Encoding.UTF8.GetString(buffer, 0, received);
                            var req = JsonSerializer.Deserialize<MultiplyRequest>(json);
                            var resultRows = MultiplyRows(req);
                            var respBytes = SerializeRows(resultRows);
                            await client.SendAsync(respBytes, SocketFlags.None);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Exception: " + ex);
                    }
                    finally
                    {
                        client.Close();
                    }
                });
            }
        }

        static MultiplyResponse MultiplyRows(MultiplyRequest req)
        {
            int rows = req.Chunk.Length;
            int n = req.OrigMatrix.Length;
            float[][] outRows = new float[rows][];
            for (int i = 0; i < rows; i++)
            {
                outRows[i] = new float[n];
                for (int j = 0; j < n; j++)
                {
                    double s = 0;
                    for (int k = 0; k < n; k++)
                    {
                        s += (double)req.Chunk[i][k] * req.OrigMatrix[k][j];
                    }
                    outRows[i][j] = (float)s;
                }
            }
            return new MultiplyResponse { RowStart = req.RowStart, Rows = outRows };
        }

        static byte[] SerializeRows(MultiplyResponse resp)
        {
            return Encoding.UTF8.GetBytes(JsonSerializer.Serialize(resp));
        }
    }
}
