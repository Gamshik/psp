using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace VideothequeTcpApp.Networking
{
    public class RestOverTcpClient
    {
        private readonly string _host; 
        private readonly int _port; 
        private readonly JsonSerializerOptions _jsonOptions; 

        public RestOverTcpClient(string host, int port)
        {
            _host = host;
            _port = port;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true 
            };
        }

        public Task<T?> GetAsync<T>(string path) => _sendRequestAsync<T>("GET", path);
        public Task<T?> PostAsync<T>(string path, T body) => _sendRequestAsync<T>("POST", path, body);
        public Task<T?> PutAsync<T>(string path, T body) => _sendRequestAsync<T>("PUT", path, body);
        public Task DeleteAsync(string path) => _sendRequestAsync<object>("DELETE", path);

        private async Task<T?> _sendRequestAsync<T>(string method, string path, object? bodyContent = null)
        {
            var requestBuilder = new StringBuilder();
            requestBuilder.AppendLine($"{method} {path}");

            byte[]? bodyBytes = null;
            if (bodyContent != null)
            {
                var jsonBody = JsonSerializer.Serialize(bodyContent);
                bodyBytes = Encoding.UTF8.GetBytes(jsonBody);
                requestBuilder.AppendLine("Content-Type: application/json");
                requestBuilder.AppendLine($"Content-Length: {jsonBody.Length}");
            }

            requestBuilder.AppendLine(); 

            using var client = new TcpClient();
            await client.ConnectAsync(_host, _port);
            await using var stream = client.GetStream();

            var headerBytes = Encoding.UTF8.GetBytes(requestBuilder.ToString());
            await stream.WriteAsync(headerBytes);

            if (bodyBytes != null)
                await stream.WriteAsync(bodyBytes);

            using var reader = new StreamReader(stream, Encoding.UTF8);
            var rawResponse = await reader.ReadToEndAsync();

            return _parseResponse<T>(rawResponse);
        }

        private T? _parseResponse<T>(string rawResponse)
        {
            if (string.IsNullOrWhiteSpace(rawResponse))
                throw new Exception("Empty respond from server");

            var parts = rawResponse.Split(new[] { "\r\n\r\n" }, 2, StringSplitOptions.None);

            var headersPart = parts[0];

            var bodyPart = parts.Length > 1 ? parts[1] : string.Empty;

            var statusLine = headersPart.Split('\r', '\n')[0];


            return statusLine switch
            {
                var s when s.StartsWith("200") || s.StartsWith("201") => JsonSerializer.Deserialize<T>(bodyPart, _jsonOptions),
                var s when s.StartsWith("204") => default,
                _ => throw new Exception($"Error: {statusLine}")
            };
        }
    }
}
