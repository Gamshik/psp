using Videotheque.CustomerService.Api.Models.DTO;

namespace Videotheque.CustomerService.Api.Application.Services.Interfaces
{
    public interface ICustomersService
    {
        // Получить всех клиентов
        IEnumerable<CustomerDTO> GetAll();

        // Получить клиента по ID
        CustomerDTO? GetById(Guid id);

        // Создать нового клиента
        CustomerDTO Create(CustomerDTO customer);

        // Обновить существующего клиента
        bool Update(CustomerDTO customer);

        // Удалить клиента по ID
        bool Delete(Guid id);
    }
}
