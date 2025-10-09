using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace VideothequeTcpApp.Networking
{
    /// <summary>
    /// Простая реализация HTTP-подобного клиента поверх TCP.
    /// Поддерживает методы GET, POST, PUT и DELETE с JSON-телом запроса/ответа.
    /// </summary>
    public class RestOverTcpClient
    {
        private readonly string _host; // Адрес сервера
        private readonly int _port; // Порт сервера
        private readonly JsonSerializerOptions _jsonOptions; // Опции сериализации JSON

        public RestOverTcpClient(string host, int port)
        {
            _host = host;
            _port = port;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true // Игнорируем регистр при десериализации
            };
        }

        // Обёртки для отправки запросов разных типов
        public Task<T?> GetAsync<T>(string path) => _sendRequestAsync<T>("GET", path);
        public Task<T?> PostAsync<T>(string path, T body) => _sendRequestAsync<T>("POST", path, body);
        public Task<T?> PutAsync<T>(string path, T body) => _sendRequestAsync<T>("PUT", path, body);
        public Task DeleteAsync(string path) => _sendRequestAsync<object>("DELETE", path);

        /// <summary>
        /// Основной метод для отправки запроса на сервер.
        /// Формирует HTTP-подобное сообщение и отправляет через TCP.
        /// </summary>
        private async Task<T?> _sendRequestAsync<T>(string method, string path, object? bodyContent = null)
        {
            // Формируем стартовую строку и заголовки
            var requestBuilder = new StringBuilder();
            requestBuilder.AppendLine($"{method} {path}");

            byte[]? bodyBytes = null;
            if (bodyContent != null)
            {
                // Сериализация тела запроса в JSON
                var jsonBody = JsonSerializer.Serialize(bodyContent);
                bodyBytes = Encoding.UTF8.GetBytes(jsonBody);
                requestBuilder.AppendLine("Content-Type: application/json");
                requestBuilder.AppendLine($"Content-Length: {jsonBody.Length}");
            }

            requestBuilder.AppendLine(); // Пустая строка разделяет заголовки и тело

            // Устанавливаем TCP-соединение
            using var client = new TcpClient();
            await client.ConnectAsync(_host, _port);
            await using var stream = client.GetStream();

            // Отправляем заголовки
            var headerBytes = Encoding.UTF8.GetBytes(requestBuilder.ToString());
            await stream.WriteAsync(headerBytes);

            // Отправляем тело, если есть
            if (bodyBytes != null)
                await stream.WriteAsync(bodyBytes);

            // Читаем весь ответ сервера
            using var reader = new StreamReader(stream, Encoding.UTF8);
            var rawResponse = await reader.ReadToEndAsync();

            // Парсим и возвращаем результат
            return _parseResponse<T>(rawResponse);
        }

        /// <summary>
        /// Разбор ответа сервера.
        /// Проверяет статусный код и десериализует JSON-тело.
        /// </summary>
        private T? _parseResponse<T>(string rawResponse)
        {
            if (string.IsNullOrWhiteSpace(rawResponse))
                throw new Exception("Empty respond from server");

            // Разделяем полученный ответ сервера на заголовки и тело
            // HTTP-подобный формат: заголовки и тело отделяются пустой строкой (\r\n\r\n)
            // Split с параметром 2 гарантирует, что мы разделим только на две части:
            // 1) заголовки (headersPart)
            // 2) тело запроса (bodyPart)
            var parts = rawResponse.Split(new[] { "\r\n\r\n" }, 2, StringSplitOptions.None);

            // Берём первую часть — это все заголовки сервера
            var headersPart = parts[0];

            // Если есть вторая часть, то это тело, иначе пустая строка
            var bodyPart = parts.Length > 1 ? parts[1] : string.Empty;

            // Первая строка заголовков содержит статус ответа сервера (например "200 OK")
            // Для этого разделяем headersPart по символам конца строки (\r и \n) и берём первый элемент
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
