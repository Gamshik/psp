using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using VideothequeTcpApp.Models.Domain;
using VideothequeTcpApp.VideoService.Application.Services.Implementations;
using VideothequeTcpApp.VideoService.Infrastructure.Persistence;

namespace VideothequeTcpApp.VideoService.Presentation.Helpers
{
    public static class ServerHelper
    {
        // Настройки сериализации JSON, игнорируем регистр имен свойств
        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        /// <summary>
        /// Метод для обработки одного подключения клиента к серверу.
        /// Разбирает запрос, выполняет CRUD-операцию через сервис и отправляет ответ.
        /// </summary>
        public static void HandleClient(TcpClient client)
        {
            Console.WriteLine("New connection!");

            // Получаем поток данных TCP-соединения
            using var stream = client.GetStream();

            // Поток для чтения текстовых данных из клиента
            using var reader = new StreamReader(stream, Encoding.UTF8);

            // Поток для записи текстового ответа клиенту
            using var writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

            AppDbContext? ctx = null; // Контекст базы данных для этого запроса
            try
            {
                // -------------------
                // 1) Читаем стартовую строку запроса (например "GET /videos")
                // -------------------
                var requestLine = reader.ReadLine();
                if (string.IsNullOrEmpty(requestLine)) return; // Если строка пустая — выходим
                Console.WriteLine(requestLine);

                // Разделяем метод (GET, POST и т.д.) и путь (/videos, /videos/{id})
                var parts = requestLine.Split(' ', 2);
                var method = parts[0];
                var path = parts.Length > 1 ? parts[1] : "/";

                // -------------------
                // 2) Читаем заголовки запроса
                // -------------------
                var headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                string? line;
                while (!string.IsNullOrEmpty(line = reader.ReadLine()))
                {
                    var headerParts = line.Split(':', 2);
                    if (headerParts.Length == 2)
                        headers[headerParts[0].Trim()] = headerParts[1].Trim(); // Добавляем заголовок в словарь
                }

                // -------------------
                // 3) Читаем тело запроса (если есть Content-Length)
                // -------------------
                string? body = null;
                if (headers.TryGetValue("Content-Length", out var contentLengthValue) &&
                    int.TryParse(contentLengthValue, out var contentLength) && contentLength > 0)
                {
                    var buffer = new char[contentLength];
                    var read = 0;

                    // Считываем ровно Content-Length символов из потока
                    while (read < contentLength)
                    {
                        var r = reader.ReadBlock(buffer, read, contentLength - read);
                        if (r == 0) break; // Конец потока
                        read += r;
                    }

                    body = new string(buffer, 0, read);
                }

                // -------------------
                // 4) Создаём контекст базы данных и сервис для выполнения операций
                // DbContext не потокобезопасен — создаём новый на каждый запрос
                // имитируем SCOPED лайфтайм
                // -------------------
                ctx = new AppDbContextFactory().CreateDbContext(Array.Empty<string>());
                var videosService = new VideosService(ctx);

                string response;

                // -------------------
                // 5) Обрабатываем запрос в зависимости от метода и пути
                // -------------------

                // GET /videos — вернуть все видео
                if (method == "GET" && path == "/videos")
                {
                    var all = videosService.GetAll();
                    response = CreateResponse("200 OK", all);
                }
                // GET /videos/{id} — вернуть конкретное видео
                else if (method == "GET" && path.StartsWith("/videos/"))
                {
                    var idPart = path.Substring("/videos/".Length);
                    if (!Guid.TryParse(idPart, out var id))
                    {
                        response = CreateResponse("400 Bad Request");
                    }
                    else
                    {
                        var video = videosService.GetById(id);
                        response = video != null ? CreateResponse("200 OK", video) : CreateResponse("404 Not Found");
                    }
                }
                // POST /videos — создать новое видео
                else if (method == "POST" && path == "/videos")
                {
                    if (string.IsNullOrEmpty(body))
                    {
                        response = CreateResponse("400 Bad Request");
                    }
                    else
                    {
                        var dto = JsonSerializer.Deserialize<Video>(body, _jsonOptions);
                        if (dto == null)
                            response = CreateResponse("400 Bad Request");
                        else
                        {
                            var created = videosService.Create(dto);
                            Console.WriteLine($"Video added: {created.Title}");
                            response = CreateResponse("201 Created", created);
                        }
                    }
                }
                // PUT /videos/{id} — обновить существующее видео
                else if (method == "PUT" && path.StartsWith("/videos/"))
                {
                    var idPart = path.Substring("/videos/".Length);
                    if (!Guid.TryParse(idPart, out var id))
                    {
                        response = CreateResponse("400 Bad Request");
                    }
                    else if (string.IsNullOrEmpty(body))
                    {
                        response = CreateResponse("400 Bad Request");
                    }
                    else
                    {
                        var dto = JsonSerializer.Deserialize<Video>(body, _jsonOptions);
                        if (dto == null)
                            response = CreateResponse("400 Bad Request");
                        else
                        {
                            // Устанавливаем ID из URL и обновляем запись
                            dto.Id = id;
                            var ok = videosService.Update(dto);
                            if (ok)
                            {
                                var updated = videosService.GetById(id);
                                response = CreateResponse("200 OK", updated);
                                Console.WriteLine($"Video updated, ID: {id}");
                            }
                            else response = CreateResponse("404 Not Found");
                        }
                    }
                }
                // DELETE /videos/{id} — удалить видео
                else if (method == "DELETE" && path.StartsWith("/videos/"))
                {
                    var idPart = path.Substring("/videos/".Length);
                    if (!Guid.TryParse(idPart, out var id))
                    {
                        response = CreateResponse("400 Bad Request");
                    }
                    else
                    {
                        var success = videosService.Delete(id);
                        if (success)
                        {
                            Console.WriteLine($"Video deleted, ID: {id}");
                            response = CreateResponse("204 No Content");
                        }
                        else response = CreateResponse("404 Not Found");
                    }
                }
                else
                {
                    // Некорректный метод или путь
                    response = CreateResponse("400 Bad Request");
                }

                // Отправляем сформированный ответ клиенту
                writer.Write(response);
            }
            catch (Exception ex)
            {
                // Логируем ошибку и отправляем код 500
                Console.WriteLine($"Error: {ex.Message}");
                writer.Write(CreateResponse("500 Internal Server Error"));
            }
            finally
            {
                // Закрываем DbContext и соединение
                ctx?.Dispose();
                client.Close();
                Console.WriteLine("Conntection closed.");
            }
        }

        // Создаёт ответ с телом в формате JSON
        private static string CreateResponse<T>(string status, T body)
        {
            var jsonBody = JsonSerializer.Serialize(body, _jsonOptions);
            return $"{status}\r\nContent-Type: application/json\r\n\r\n{jsonBody}";
        }

        // Создаёт ответ без тела (например 204 No Content)
        private static string CreateResponse(string status) => $"{status}\r\n\r\n";
    }
}
