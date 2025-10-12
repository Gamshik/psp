using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Web;

namespace Videotheque.ReportingService.Api.Presentation.Helpers
{
    public static class ServerHelper
    {
        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            AllowTrailingCommas = true,
            ReadCommentHandling = JsonCommentHandling.Skip
        };

        public static async Task HandleClientAsync(TcpClient client)
        {
            Console.WriteLine($"ReportingService received request at {DateTime.Now:yyyy-MM-dd HH:mm:ss} for path: {client.Client.RemoteEndPoint}");

            using var stream = client.GetStream();
            using var reader = new StreamReader(stream, new UTF8Encoding(false));
            using var writer = new StreamWriter(stream, new UTF8Encoding(false)) { AutoFlush = true };

            try
            {
                string? requestLine = await reader.ReadLineAsync();
                if (string.IsNullOrEmpty(requestLine))
                {
                    Console.WriteLine("Empty request line, closing connection.");
                    return;
                }
                Console.WriteLine($"Request: {requestLine}");

                var parts = requestLine.Split(' ', 2);
                var method = parts.Length > 0 ? parts[0] : "";
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
                    var totalRead = 0;
                    while (totalRead < contentLength)
                    {
                        var read = await reader.ReadAsync(buffer, totalRead, contentLength - totalRead);
                        if (read == 0) break;
                        totalRead += read;
                    }
                    if (totalRead != contentLength)
                    {
                        Console.WriteLine($"Warning: Read {totalRead} bytes, expected {contentLength}");
                    }
                    body = new string(buffer, 0, totalRead);
                }

                var reportingService = new Application.Services.Implementations.ReportingService();
                string response;

                if (method == "GET" && path.StartsWith("/reports"))
                {
                    var queryParams = HttpUtility.ParseQueryString(new Uri("http://edu.gstu.com" + path).Query);
                    var fromStr = queryParams["from"];
                    var toStr = queryParams["to"];
                    if (DateTime.TryParse(fromStr, out var from) && DateTime.TryParse(toStr, out var to))
                    {
                        var report = await reportingService.GetReportAsync(from, to);
                        Console.WriteLine($"Report generated: TotalRevenue={report.TotalRevenue}");
                        response = CreateResponse("200 OK", report);
                    }
                    else
                    {
                        response = CreateResponse("400 Bad Request");
                    }
                }
                else
                {
                    response = CreateResponse("400 Bad Request");
                }

                Console.WriteLine($"Preparing to send response: Status={response.Split('\r', '\n')[0]}, Length={response.Length}");
                if (string.IsNullOrEmpty(response))
                {
                    Console.WriteLine("Warning: Response is null or empty, sending default 500");
                    response = CreateResponse("500 Internal Server Error");
                }
                Console.WriteLine($"Sending response: {response.Substring(0, Math.Min(100, response.Length))}...");
                await writer.WriteAsync(response); 
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                if (ex.InnerException != null)
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                await writer.WriteAsync(CreateResponse("500 Internal Server Error"));
            }
            finally
            {
                client.Close();
                Console.WriteLine("Connection closed.");
            }
        }

        private static string CreateResponse<T>(string status, T body)
        {
            var jsonBody = body != null ? JsonSerializer.Serialize(body, _jsonOptions) : "[]";
            return $"{status}\r\nContent-Type: application/json\r\nContent-Length: {jsonBody.Length}\r\n\r\n{jsonBody}";
        }

        private static string CreateResponse(string status) => $"{status}\r\n\r\n";
    }
}