using Videotheque.RentalService.Api.Application.Services.Interfaces;
using Videotheque.RentalService.Api.Infrastructure.Persistence;
using Videotheque.RentalService.Api.Models.Domain;
using Videotheque.RentalService.Api.Models.DTO;
using VideothequeTcpApp.Networking;

namespace Videotheque.RentalService.Api.Application.Services.Implementations
{
    public class RentalsService : IRentalsService
    {
        private readonly AppDbContext _context;
        private readonly RestOverTcpClient _videoClient; 
        private readonly RestOverTcpClient _customerClient;

        public RentalsService(AppDbContext context)
        {
            _context = context;
            _videoClient = new RestOverTcpClient("127.0.0.1", 9001); 
            _customerClient = new RestOverTcpClient("127.0.0.1", 9002); 
        }

        public RentalsService() : this(new AppDbContextFactory().CreateDbContext(Array.Empty<string>()))
        {
        }

        public IEnumerable<RentalDTO> GetAll()
        {
            return _context.Rentals.Select(r => new RentalDTO
            {
                Id = r.Id,
                CustomerId = r.CustomerId,
                VideoId = r.VideoId,
                StartDate = r.StartDate,
                EndDate = r.EndDate,
                IsReturned = r.IsReturned,
                TotalPrice = r.TotalPrice
            }).ToList();
        }

        public RentalDTO? GetById(Guid id)
        {
            var rental = _context.Rentals.FirstOrDefault(r => r.Id == id);
            return rental != null ? MapToDTO(rental) : null;
        }

        public async Task<RentalDTO> CreateAsync(RentalDTO rentalDTO)
        {
            var customer = await _customerClient.GetAsync<CustomerDTO>("customers/" + rentalDTO.CustomerId);
            if (customer == null) throw new Exception("Customer not found");

            
            var video = await _videoClient.GetAsync<VideoDTO>("videos/" + rentalDTO.VideoId);
            if (video == null) throw new Exception("Video not found");
            var rental = new Rental
            {
                Id = Guid.NewGuid(),
                CustomerId = rentalDTO.CustomerId,
                VideoId = rentalDTO.VideoId,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(7),
                IsReturned = false,
                TotalPrice = video.Price
            };

            _context.Rentals.Add(rental);
            _context.SaveChanges();
            return MapToDTO(rental);
        }

        public bool Update(RentalDTO rentalDTO)
        {
            var existing = _context.Rentals.FirstOrDefault(r => r.Id == rentalDTO.Id);
            if (existing == null) return false;

            existing.EndDate = rentalDTO.EndDate;
            existing.IsReturned = rentalDTO.IsReturned;
            existing.TotalPrice = rentalDTO.TotalPrice;
            _context.SaveChanges();
            return true;
        }

        public bool Delete(Guid id)
        {
            var existing = _context.Rentals.FirstOrDefault(r => r.Id == id);
            if (existing == null) return false;

            _context.Rentals.Remove(existing);
            _context.SaveChanges();
            return true;
        }

        public async Task<IEnumerable<RentalDTO>> GetRentalsByDateAsync(DateTime from, DateTime to)
        {
            var rentals = _context.Rentals
                .Where(r => r.StartDate >= from && r.EndDate <= to)
                .Select(r => new RentalDTO
                {
                    Id = r.Id,
                    CustomerId = r.CustomerId,
                    VideoId = r.VideoId,
                    StartDate = r.StartDate,
                    EndDate = r.EndDate,
                    IsReturned = r.IsReturned,
                    TotalPrice = r.TotalPrice
                });
            return await Task.FromResult(rentals.ToList());
        }

        private RentalDTO MapToDTO(Rental rental)
        {
            return new RentalDTO
            {
                Id = rental.Id,
                CustomerId = rental.CustomerId,
                VideoId = rental.VideoId,
                StartDate = rental.StartDate,
                EndDate = rental.EndDate,
                IsReturned = rental.IsReturned,
                TotalPrice = rental.TotalPrice
            };
        }
    }
}