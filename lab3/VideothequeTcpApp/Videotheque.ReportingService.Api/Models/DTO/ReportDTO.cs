namespace Videotheque.ReportingService.Api.Models.DTO
{
    public class ReportDTO
    {
        public decimal TotalRevenue { get; set; }
        public Guid MostPopularVideoId { get; set; }
        public int MostPopularVideoRentals { get; set; }
        public Guid MostProfitableVideoId { get; set; }
        public decimal MostProfitableVideoRevenue { get; set; }
        public double AverageRentalDuration { get; set; }
        public Guid TopCustomerId { get; set; }
        public decimal TopCustomerSpent { get; set; }
    }
}
