using Videotheque.ReportingService.Api.Application.Services.Interfaces;
using Videotheque.ReportingService.Api.Models.DTO;
using VideothequeTcpApp.Networking;

namespace Videotheque.ReportingService.Api.Application.Services.Implementations
{

    public class ReportingService : IReportingService
    {
        private readonly RestOverTcpClient _rentalClient; 

        public ReportingService()
        {
            _rentalClient = new RestOverTcpClient("127.0.0.1", 9003); 
        }

        public async Task<ReportDTO> GetReportAsync(DateTime from, DateTime to)
        {
            var rentals = await GetRentalsAsync(from, to);

            var totalRevenue = rentals.Sum(r => r.TotalPrice);
            var popularVideo = rentals
                .GroupBy(r => r.VideoId)
                .Select(g => new { VideoId = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .FirstOrDefault();
            var profitableVideo = rentals
                .GroupBy(r => r.VideoId)
                .Select(g => new { VideoId = g.Key, TotalRevenue = g.Sum(r => r.TotalPrice) })
                .OrderByDescending(x => x.TotalRevenue)
                .FirstOrDefault();
            var completedRentals = rentals.Where(r => r.IsReturned && r.EndDate > r.StartDate);
            var averageDays = completedRentals.Any()
                ? completedRentals.Average(r => (r.EndDate - r.StartDate).TotalDays)
                : 0;
            var topCustomer = rentals
                .GroupBy(r => r.CustomerId)
                .Select(g => new { CustomerId = g.Key, TotalSpent = g.Sum(r => r.TotalPrice) })
                .OrderByDescending(x => x.TotalSpent)
                .FirstOrDefault();

            return new ReportDTO
            {
                TotalRevenue = totalRevenue,
                MostPopularVideoId = popularVideo?.VideoId ?? Guid.Empty,
                MostPopularVideoRentals = popularVideo?.Count ?? 0,
                MostProfitableVideoId = profitableVideo?.VideoId ?? Guid.Empty,
                MostProfitableVideoRevenue = profitableVideo?.TotalRevenue ?? 0m,
                AverageRentalDuration = averageDays,
                TopCustomerId = topCustomer?.CustomerId ?? Guid.Empty,
                TopCustomerSpent = topCustomer?.TotalSpent ?? 0m
            };
        }

        private async Task<List<RentalDTO>> GetRentalsAsync(DateTime from, DateTime to)
        {
            var query = $"/rentals?from={from:yyyy-MM-dd}&to={to:yyyy-MM-dd}";
            return await _rentalClient.GetAsync<List<RentalDTO>>(query) ?? new List<RentalDTO>();
        }
    }
}