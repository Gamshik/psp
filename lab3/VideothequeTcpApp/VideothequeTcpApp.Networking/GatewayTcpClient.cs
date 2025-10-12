using System.Net.Sockets;
using System.Text;

namespace VideothequeTcpApp.Networking
{
    public class GatewayTcpClient
    {
        private readonly string _host;
        private readonly int _port;

        public GatewayTcpClient(string host, int port)
        {
            _host = host;
            _port = port;
        }

        public async Task<string> GetAsync(string path) => await _sendRequestAsync("GET", path, null);
        public async Task<string> PostAsync(string path, string? rawBody = null) => await _sendRequestAsync("POST", path, rawBody);
        public async Task<string> PutAsync(string path, string? rawBody = null) => await _sendRequestAsync("PUT", path, rawBody);
        public async Task<string> DeleteAsync(string path) => await _sendRequestAsync("DELETE", path, null);

        private async Task<string> _sendRequestAsync(string method, string path, string? rawBody = null)
        {
            var requestBuilder = new StringBuilder();
            requestBuilder.AppendLine($"{method} {path}");
            requestBuilder.AppendLine($"Host: {_host}");

            byte[]? bodyBytes = null;
            if (rawBody != null)
            {
                bodyBytes = Encoding.UTF8.GetBytes(rawBody);
                requestBuilder.AppendLine("Content-Type: application/json; charset=utf-8");
                requestBuilder.AppendLine($"Content-Length: {rawBody.Length}");
            }
            else
            {
                requestBuilder.AppendLine("Content-Length: 0");
            }

            requestBuilder.AppendLine();

            using var client = new TcpClient();
            await client.ConnectAsync(_host, _port);
            await using var stream = client.GetStream();

            var headerBytes = Encoding.UTF8.GetBytes(requestBuilder.ToString());
            await stream.WriteAsync(headerBytes);

            if (bodyBytes != null)
                await stream.WriteAsync(bodyBytes);

            using var reader = new StreamReader(stream, new UTF8Encoding(false));
            var rawResponse = await reader.ReadToEndAsync();

            var parts = rawResponse.Split(new[] { "\r\n\r\n" }, 2, StringSplitOptions.None);
            if (parts.Length > 0)
            {
                var statusLine = parts[0].Split('\r', '\n')[0];
                if (!statusLine.StartsWith("2"))
                    return CreateResponse("500 Internal Server Error");
            }

            return rawResponse;
        }

        private string CreateResponse(string status)
        {
            return $"{status}\r\n\r\n";
        }
    }
}