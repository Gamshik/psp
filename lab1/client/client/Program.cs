using client.dto;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace Client;

internal class Program
{
    static async Task Main(string[] args)
    {
        string serversArg = "127.0.0.1:5000,127.0.0.1:5001,127.0.0.1:5002";
        int n = 4;
        int power = 3;
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "--servers" && i + 1 < args.Length) serversArg = args[++i];
            if (args[i] == "--n" && i + 1 < args.Length) int.TryParse(args[++i], out n);
            if (args[i] == "--m" && i + 1 < args.Length) int.TryParse(args[++i], out power);
        }

        var serverEndpoints = serversArg.Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(s =>
            {
                var parts = s.Split(':');
                return new IPEndPoint(IPAddress.Parse(parts[0]), int.Parse(parts[1]));
            }).ToArray();

        var sockets = new List<Socket>();
        foreach (var ep in serverEndpoints)
        {
            var sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            await sock.ConnectAsync(ep);
            sockets.Add(sock);
            Console.WriteLine($"Connected to {ep}");
        }

        var random = new Random();
        float[][] A = new float[n][];
        for (int i = 0; i < n; i++)
        {
            A[i] = new float[n];
            for (int j = 0; j < n; j++) A[i][j] = (float)random.Next(1, 5);
        }

        Console.WriteLine("Matrix A:");
        PrintMatrix(A);

        Stopwatch sw = Stopwatch.StartNew();
        float[][] result = await DistributedPower(A, power, sockets);
        sw.Stop();
        Console.WriteLine($"Result A^{power}:");
        PrintMatrix(result);
        Console.WriteLine($"Elapsed ms: {sw.ElapsedMilliseconds}");

        foreach (var s in sockets) s.Close();
    }

    static async Task<float[][]> DistributedPower(float[][] A, int power, List<Socket> sockets)
    {
        int n = A.Length;
        float[][] cur = CloneMatrix(A);
        for (int t = 1; t <= power - 1; t++)
        {
            cur = await DistributedMultiply(cur, A, sockets);
        }
        return cur;
    }

    static async Task<float[][]> DistributedMultiply(float[][] curr, float[][] origMatrix, List<Socket> sockets)
    {
        int n = curr.Length;
        int k = sockets.Count;
        int baseRows = n / k;
        int remain = n % k;

        var tasks = new List<Task<ServerResponse>>();
        int rowCursor = 0;
        for (int i = 0; i < k; i++)
        {
            int rowsForThis = baseRows + (i < remain ? 1 : 0);
            if (rowsForThis == 0) { tasks.Add(Task.FromResult(new ServerResponse { RowStart = rowCursor, Rows = new float[0][] })); continue; }
            var chunk = new float[rowsForThis][];
            for (int r = 0; r < rowsForThis; r++) chunk[r] = curr[rowCursor + r];
            var req = new ClientRequest { Chunk = chunk, OrigMatrix = origMatrix, RowStart = rowCursor };
            tasks.Add(SendRequestAsync(sockets[i], req));
            rowCursor += rowsForThis;
        }

        var responses = await Task.WhenAll(tasks);

        var result = new float[n][];
        foreach (var resp in responses)
        {
            if (resp.Rows == null) continue;
            int rs = resp.RowStart;
            for (int i = 0; i < resp.Rows.Length; i++)
            {
                result[rs + i] = resp.Rows[i];
            }
        }
        return result;
    }

    static async Task<ServerResponse> SendRequestAsync(Socket socket, ClientRequest req)
    {
        var json = JsonSerializer.Serialize(req);
        var bytes = Encoding.UTF8.GetBytes(json);
        await socket.SendAsync(bytes, SocketFlags.None);

        var buffer = new byte[10 * 1024 * 1024];
        int rec = await socket.ReceiveAsync(buffer, SocketFlags.None);
        var respJson = Encoding.UTF8.GetString(buffer, 0, rec);
        var resp = JsonSerializer.Deserialize<ServerResponse>(respJson);
        return resp;
    }

    static float[][] CloneMatrix(float[][] m) => m.Select(r => r.ToArray()).ToArray();

    static void PrintMatrix(float[][] m)
    {
        for (int i = 0; i < m.Length; i++)
        {
            Console.WriteLine(string.Join(' ', m[i].Select(x => x.ToString("F2"))));
        }
    }
}
