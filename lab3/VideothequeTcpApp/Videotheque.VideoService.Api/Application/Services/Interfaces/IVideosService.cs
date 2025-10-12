using Videotheque.VideoService.Api.Models.DTO;

namespace Videotheque.VideoService.Api.Application.Services.Interfaces
{
    public interface IVideosService
    {
        // Получить все видео из базы данных
        IEnumerable<VideoDTO> GetAll();

        // Получить одно видео по идентификатору
        VideoDTO? GetById(Guid id);

        // Создать новое видео и добавить его в базу
        VideoDTO Create(VideoDTO video);

        // Обновить существующее видео; возвращает true если обновление прошло успешно
        bool Update(VideoDTO video);

        // Удалить видео по идентификатору; возвращает true если удаление прошло успешно
        bool Delete(Guid id);
    }
}
