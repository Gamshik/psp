using System.Net;
using System.Net.Sockets;
using System.Text;
using VideothequeTcpApp.Networking;

class Program
{
    private static readonly Dictionary<string, (string Host, int Port, string BasePath)> _routes = new()
    {
        { "/api/videos", ("127.0.0.1", 9001, "/videos") },
        { "/api/customers", ("127.0.0.1", 9002, "/customers") },
        { "/api/rentals", ("127.0.0.1", 9003, "/rentals") },
        { "/api/reports", ("127.0.0.1", 9004, "/reports") }
    };

    static void Main(string[] args)
    {
        const int gatewayPort = 8888;
        var listener = new TcpListener(IPAddress.Any, gatewayPort);
        listener.Start();
        Console.WriteLine($"API Gateway listening on port {gatewayPort}");

        while (true)
        {
            var client = listener.AcceptTcpClient();
            _ = Task.Run(() => HandleClient(client));
        }
    }

    static async Task HandleClient(TcpClient client)
    {
        Console.WriteLine($"New client connected at {DateTime.Now:yyyy-MM-dd HH:mm:ss}");

        using var stream = client.GetStream();
        using var reader = new StreamReader(stream, new UTF8Encoding(false));
        using var writer = new StreamWriter(stream, new UTF8Encoding(false)) { AutoFlush = true };

        try
        {
            var requestLine = await reader.ReadLineAsync();
            if (string.IsNullOrEmpty(requestLine)) return;
            Console.WriteLine($"Request: {requestLine}");

            var parts = requestLine.Split(' ', 2);
            var method = parts[0];
            var path = parts.Length > 1 ? parts[1] : "/";

            var headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            string? line;
            while (!string.IsNullOrEmpty(line = await reader.ReadLineAsync()) && line != "\r\n")
            {
                var headerParts = line.Split(':', 2);
                if (headerParts.Length == 2)
                    headers[headerParts[0].Trim()] = headerParts[1].Trim();
            }
            Console.WriteLine($"Headers: {string.Join(", ", headers.Select(h => $"{h.Key}: {h.Value}"))}");

            string? body = null;
            if (headers.TryGetValue("Content-Length", out var contentLengthValue) &&
                int.TryParse(contentLengthValue, out var contentLength) &&
                contentLength > 0)
            {
                var buffer = new char[contentLength];
                var read = await reader.ReadAsync(buffer, 0, contentLength);
                if (read != contentLength)
                {
                    Console.WriteLine($"Warning: Read {read} bytes, expected {contentLength}");
                }
                body = new string(buffer, 0, read);
                Console.WriteLine($"Received body (length {read}): {body}");
            }

            var route = _routes.FirstOrDefault(r => path.StartsWith(r.Key));
            if (string.IsNullOrEmpty(route.Key))
            {
                writer.Write(CreateResponse("404 Not Found"));
                return;
            }

            var (host, port, basePath) = route.Value;
            var targetPath = string.IsNullOrEmpty(path.Substring(route.Key.Length)) ? basePath : $"{basePath}{path.Substring(route.Key.Length)}";

            string response;
            var clientToService = new GatewayTcpClient(host, port);
            switch (method.ToUpper())
            {
                case "GET":
                    Console.WriteLine($"Sending GET to {host}:{port}{targetPath}");
                    response = await clientToService.GetAsync(targetPath);
                    Console.WriteLine($"Received response: {response}");
                    break;
                case "POST":
                    Console.WriteLine($"Sending POST to {host}:{port}{targetPath} with body: {body}");
                    response = await clientToService.PostAsync(targetPath, body);
                    break;
                case "PUT":
                    Console.WriteLine($"Sending PUT to {host}:{port}{targetPath} with body: {body}");
                    response = await clientToService.PutAsync(targetPath, body);
                    break;
                case "DELETE":
                    await clientToService.DeleteAsync(targetPath);
                    response = CreateResponse("204 No Content");
                    break;
                default:
                    response = CreateResponse("400 Bad Request");
                    break;
            }

            writer.Write(response);
            Console.WriteLine($"Routed to {host}:{port}, path: {targetPath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            writer.Write(CreateResponse("500 Internal Server Error"));
        }
        finally
        {
            client.Close();
            Console.WriteLine("Client connection closed.");
        }
    }

    static string CreateResponse(string status)
    {
        return $"{status}\r\n\r\n";
    }
}