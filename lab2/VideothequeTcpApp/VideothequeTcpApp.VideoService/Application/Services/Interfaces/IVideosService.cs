using VideothequeTcpApp.Models.Domain;

namespace VideothequeTcpApp.VideoService.Application.Services.Interfaces
{
    public interface IVideosService
    {
        // Получить все видео из базы данных
        IEnumerable<Video> GetAll();

        // Получить одно видео по идентификатору
        Video? GetById(Guid id);

        // Создать новое видео и добавить его в базу
        Video Create(Video video);

        // Обновить существующее видео; возвращает true если обновление прошло успешно
        bool Update(Video video);

        // Удалить видео по идентификатору; возвращает true если удаление прошло успешно
        bool Delete(Guid id);
    }
}
