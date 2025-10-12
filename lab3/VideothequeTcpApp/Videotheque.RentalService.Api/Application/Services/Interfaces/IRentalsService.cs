using Videotheque.RentalService.Api.Models.DTO;

namespace Videotheque.RentalService.Api.Application.Services.Interfaces
{
    public interface IRentalsService
    {
        IEnumerable<RentalDTO> GetAll();
        RentalDTO? GetById(Guid id);
        Task<RentalDTO> CreateAsync(RentalDTO rental);
        bool Update(RentalDTO rental);
        bool Delete(Guid id);
        Task<IEnumerable<RentalDTO>> GetRentalsByDateAsync(DateTime from, DateTime to); // Новый метод
    }
}