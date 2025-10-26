using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace HttpsServer;

public class HttpsServer(string ipAddress, int port, string certificatePath, string certificatePassword)
{
    private readonly TcpListener _listener = new(IPAddress.Parse(ipAddress), port);
    private readonly X509Certificate2 _serverCertificate = new(certificatePath, certificatePassword);
    
    private const SslProtocols SupportedSslProtocols = SslProtocols.Tls12;

    public void Start()
    {
        Console.WriteLine($"Запуск HTTPS-сервера на {ipAddress}:{port}...");
        try
        {
            _listener.Start();
            Console.WriteLine("Сервер запущен. Ожидание подключений...");

            _ = AcceptConnectionsAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Критическая ошибка при запуске: {ex.Message}");
            _listener.Stop();
        }
    }
    
    private async Task AcceptConnectionsAsync()
    {
        while (true)
        {
            try
            {
                TcpClient client = await _listener.AcceptTcpClientAsync();

                _ = HandleClientAsync(client);
            }
            catch (OperationCanceledException)
            {
                break; 
            }
            catch (ObjectDisposedException)
            {
                break;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при приёме клиента: {ex.Message}");
            }
        }
    }

    private async Task HandleClientAsync(TcpClient client)
    {
        using (client)
        {
            using var sslStream = new SslStream(client.GetStream(), false);

            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Новое соединение от {client.Client.RemoteEndPoint}");

            try
            {
                await sslStream.AuthenticateAsServerAsync(_serverCertificate,
                    clientCertificateRequired: false,
                    enabledSslProtocols: SupportedSslProtocols,
                    checkCertificateRevocation: false);

                using var reader = new StreamReader(sslStream, Encoding.UTF8, true, bufferSize: 1024, leaveOpen: true);

                var (requestLine, headers) = await ParseHttpRequest(reader);

                if (string.IsNullOrEmpty(requestLine))
                {
                    return; 
                }

                Console.WriteLine("\n--- Получен запрос ---");
                Console.WriteLine(requestLine);
                foreach (var header in headers)
                {
                    Console.WriteLine($"{header.Key}: {header.Value}");
                }

                var requestParts = requestLine.Split(' ');
                string method = requestParts[0];
                string path = requestParts[1];

                if (method == "GET" && path == "/")
                {
                    Console.WriteLine("--- Конец запроса ---\n");
                    await HandleGetRequestAsync(sslStream);
                }
                else if (method == "POST" && path == "/")
                {
                    string body = await ReadRequestBodyAsync(reader, headers);

                    if (body.StartsWith("Error: "))
                    {
                        Console.WriteLine($"Ошибка при чтении тела: {body}");
                        await SendHttpResponseAsync(sslStream, "400 Bad Request", "text/html; charset=utf-8", HtmlTemplates.BadRequestHtml);
                    }
                    else
                    {
                        Console.WriteLine("\n--- Тело запроса ---");
                        Console.WriteLine(body);
                        Console.WriteLine("--- Конец запроса ---\n");

                        await HandlePostRequestAsync(sslStream, body);
                    }
                }
                else
                {
                    await SendNotFoundResponseAsync(sslStream);
                }
            }
            catch (AuthenticationException e)
            {
                Console.WriteLine($"Ошибка аутентификации: {e.Message}");
            }
            catch (IOException e) when (e.InnerException is SocketException)
            {
                Console.WriteLine($"Соединение разорвано клиентом: {e.InnerException.Message}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Произошла ошибка обработки клиента: {e.Message}");
            }
            finally
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Соединение закрыто.");
            }
        }
    }

    private static async Task<(string RequestLine, Dictionary<string, string> Headers)> ParseHttpRequest(StreamReader reader)
    {
        string? requestLine = await reader.ReadLineAsync();

        if (string.IsNullOrEmpty(requestLine))
        {
            return (string.Empty, new Dictionary<string, string>());
        }

        var headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        string? headerLine;

        while (!string.IsNullOrEmpty(headerLine = await reader.ReadLineAsync()))
        {
            var parts = headerLine.Split([':'], 2);
            if (parts.Length == 2)
            {
                headers[parts[0].Trim()] = parts[1].Trim();
            }
        }

        return (requestLine, headers);
    }

    private static async Task<string> ReadRequestBodyAsync(StreamReader reader, Dictionary<string, string> headers)
    {
        if (!headers.TryGetValue("Content-Length", out var contentLengthStr) ||
            !int.TryParse(contentLengthStr, out int contentLength) ||
            contentLength <= 0)
        {
            return "Error: Недопустимый или отсутствующий Content-Length.";
        }
        
        var buffer = new char[contentLength];
        
        int bytesRead = await reader.ReadAsync(buffer, 0, contentLength);

        if (bytesRead != contentLength)
        {
            return "Error: Не удалось считать полное тело запроса.";
        }

        return new string(buffer);
    }

    private async Task HandleGetRequestAsync(SslStream sslStream)
    {
        await SendHttpResponseAsync(sslStream, "200 OK", "text/html; charset=utf-8", HtmlTemplates.InputFormHtml);
    }

    private async Task HandlePostRequestAsync(SslStream sslStream, string body)
    {
        var formData = ParseFormData(body);

        formData.TryGetValue("expression", out string? expressionStr);

        expressionStr ??= "";

        string? errorMessage = null;
        string resultHtml = "";

        try
        {
            if (string.IsNullOrWhiteSpace(expressionStr))
            {
                errorMessage = "Ошибка: Выражение не может быть пустым.";
            }
            else
            {
                expressionStr = expressionStr.Replace(" ", "")
                             .Replace("\n", "")
                             .Replace("\r", "")
                             .Trim();

                double resultValue = ExpressionCalculator.Evaluate(expressionStr);
               
                resultHtml = $@"
                <p><strong>Исходное выражение:</strong> <code>{expressionStr}</code></p>
                <hr>
                <h2>Результат вычисления:</h2>
                <div class='result'>
                    <p><strong>Значение =</strong> {resultValue.ToString("F4", CultureInfo.InvariantCulture)}</p>
                </div>";
            }
        }
        catch (ArgumentException ex)
        {
            errorMessage = $"Ошибка в выражении: {ex.Message}";
        }
        catch (Exception ex)
        {
            errorMessage = $"Произошла ошибка при вычислении: {ex.Message}";
        }

        string finalHtml = HtmlTemplates.GenerateResultHtml(errorMessage, resultHtml);

        await SendHttpResponseAsync(sslStream, "200 OK", "text/html; charset=utf-8", finalHtml);
    }

    private static Dictionary<string, string> ParseFormData(string body)
    {
        var result = new Dictionary<string, string>(StringComparer.Ordinal);

        if (string.IsNullOrEmpty(body))
            return result;

        var pairs = body.Split('&');
        foreach (var pair in pairs)
        {
            var keyValue = pair.Split('=', (char)2);
            if (keyValue.Length == 2)
            {
                string key = HttpUtility.UrlDecode(keyValue[0]);
                string value = HttpUtility.UrlDecode(keyValue[1]);
                key = key.Trim();
                value = value.Trim();

                result[key] = value;
            }
        }

        return result;
    }

    private async Task SendHttpResponseAsync(SslStream stream, string statusCode, string contentType, string content)
    {
        byte[] contentBytes = Encoding.UTF8.GetBytes(content);
        var responseBuilder = new StringBuilder();

        responseBuilder.AppendLine($"HTTP/1.1 {statusCode}");
        responseBuilder.AppendLine($"Content-Type: {contentType}");
        responseBuilder.AppendLine($"Content-Length: {contentBytes.Length}");
        responseBuilder.AppendLine("Connection: close");
        responseBuilder.AppendLine();

        string headers = responseBuilder.ToString();
        byte[] headerBytes = Encoding.UTF8.GetBytes(headers);

        await stream.WriteAsync(headerBytes, 0, headerBytes.Length); 
        await stream.WriteAsync(contentBytes, 0, contentBytes.Length);
        await stream.FlushAsync();
    }

    private async Task SendNotFoundResponseAsync(SslStream sslStream)
    {
        await SendHttpResponseAsync(sslStream, "404 Not Found", "text/html; charset=utf-8", HtmlTemplates.NotFoundHtml);
    }
}