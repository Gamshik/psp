using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using Videotheque.VideoService.Api.Application.Services.Implementations;
using Videotheque.VideoService.Api.Infrastructure.Persistence;
using Videotheque.VideoService.Api.Models.DTO;

namespace Videotheque.VideoService.Api.Presentation.Helpers
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
            Console.WriteLine($"VideoService received request at {DateTime.Now:yyyy-MM-dd HH:mm:ss} for path: {client.Client.RemoteEndPoint}");

            using var stream = client.GetStream();
            using var reader = new StreamReader(stream, new UTF8Encoding(false));
            using var writer = new StreamWriter(stream, new UTF8Encoding(false)) { AutoFlush = true };

            AppDbContext? ctx = null;

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

                ctx = new AppDbContextFactory().CreateDbContext(Array.Empty<string>());
                var videosService = new VideosService(ctx);

                string response;
                if (method == "GET" && path == "/videos")
                {
                    var all = videosService.GetAll();
                    Console.WriteLine($"Retrieved {all?.Count() ?? 0} videos");
                    response = CreateResponse("200 OK", all);
                }
                else if (method == "GET" && path.StartsWith("videos/"))
                {
                    var idPart = path.Substring("videos/".Length);
                    if (!Guid.TryParse(idPart, out var id))
                        response = CreateResponse("400 Bad Request");
                    else
                    {
                        var video = videosService.GetById(id);
                        response = video != null ? CreateResponse("200 OK", video) : CreateResponse("404 Not Found");
                    }
                }
                else if (method == "POST" && path == "/videos")
                {
                    if (string.IsNullOrEmpty(body))
                        response = CreateResponse("400 Bad Request");
                    else
                    {
                        try
                        {
                            var dto = JsonSerializer.Deserialize<VideoDTO>(body, _jsonOptions);
                            if (dto == null)
                            {
                                Console.WriteLine($"Deserialization failed. Body length: {body.Length}, Content: {body}");
                                response = CreateResponse("400 Bad Request");
                            }
                            else
                            {
                                var created = videosService.Create(dto);
                                Console.WriteLine($"Video created: {created?.Title}");
                                response = created != null ? CreateResponse("201 Created", created) : CreateResponse("400 Bad Request");
                            }
                        }
                        catch (JsonException ex)
                        {
                            Console.WriteLine($"JSON deserialization error: {ex.Message}, Body: {body}");
                            response = CreateResponse("400 Bad Request");
                        }
                    }
                }
                else if (method == "PUT" && path.StartsWith("/videos/"))
                {
                    var idPart = path.Substring("/videos/".Length);
                    if (!Guid.TryParse(idPart, out var id))
                        response = CreateResponse("400 Bad Request");
                    else if (string.IsNullOrEmpty(body))
                        response = CreateResponse("400 Bad Request");
                    else
                    {
                        var dto = JsonSerializer.Deserialize<VideoDTO>(body, _jsonOptions);
                        if (dto == null)
                            response = CreateResponse("400 Bad Request");
                        else
                        {
                            dto.Id = id;
                            var ok = videosService.Update(dto);
                            if (ok)
                            {
                                var updated = videosService.GetById(id);
                                response = updated != null ? CreateResponse("200 OK", updated) : CreateResponse("404 Not Found");
                                Console.WriteLine($"Video updated: {id}");
                            }
                            else response = CreateResponse("404 Not Found");
                        }
                    }
                }
                else if (method == "DELETE" && path.StartsWith("/videos/"))
                {
                    var idPart = path.Substring("/videos/".Length);
                    if (!Guid.TryParse(idPart, out var id))
                        response = CreateResponse("400 Bad Request");
                    else
                    {
                        var success = videosService.Delete(id);
                        if (success)
                        {
                            Console.WriteLine($"Video deleted: {id}");
                            response = CreateResponse("204 No Content");
                        }
                        else
                        {
                            Console.WriteLine($"Delete failed for id: {id}, entity not found");
                            response = CreateResponse("404 Not Found");
                        }
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
                ctx?.Dispose();
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