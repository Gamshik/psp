namespace Videotheque.RentalService.Api.Models.DTO
{
    public class RentalDTO
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public Guid VideoId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsReturned { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
