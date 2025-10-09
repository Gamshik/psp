using VideothequeTcpApp.Models.Domain;
using VideothequeTcpApp.VideoService.Application.Services.Interfaces;
using VideothequeTcpApp.VideoService.Infrastructure.Persistence;

namespace VideothequeTcpApp.VideoService.Application.Services.Implementations
{
    public class VideosService : IVideosService
    {
        private readonly AppDbContext _context;

        // Конструктор с внедрением зависимостей DbContext
        public VideosService(AppDbContext context)
        {
            _context = context;
        }

        // Параметрless конструктор, создающий новый DbContext через фабрику
        // Полезен для ручного использования сервиса вне DI
        public VideosService() : this(new AppDbContextFactory().CreateDbContext(Array.Empty<string>()))
        {
        }

        // Получение всех видео
        public IEnumerable<Video> GetAll()
        {
            return _context.Videos.ToList(); // ToList() чтобы материализовать данные сразу
        }

        // Получение видео по ID
        public Video? GetById(Guid id)
        {
            return _context.Videos.FirstOrDefault(v => v.Id == id);
        }

        // Создание нового видео
        public Video Create(Video video)
        {
            video.Id = Guid.NewGuid(); // Генерация уникального ID
            _context.Videos.Add(video); // Добавление в контекст
            _context.SaveChanges();     // Сохранение изменений в базе
            return video;
        }

        // Обновление существующего видео
        public bool Update(Video video)
        {
            var existing = _context.Videos.FirstOrDefault(v => v.Id == video.Id);
            if (existing == null) return false;

            // Обновляем свойства
            existing.Title = video.Title;
            existing.Genre = video.Genre;
            existing.Year = video.Year;
            existing.Director = video.Director;
            existing.Price = video.Price;

            _context.SaveChanges(); // Сохраняем изменения в базе
            return true;
        }

        // Удаление видео по ID
        public bool Delete(Guid id)
        {
            var existing = _context.Videos.FirstOrDefault(v => v.Id == id);
            if (existing == null) return false;

            _context.Videos.Remove(existing); // Удаление из контекста
            _context.SaveChanges();           // Сохраняем изменения в базе
            return true;
        }
    }
}
