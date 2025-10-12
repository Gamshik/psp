namespace VideothequeTcpApp.Wpf.Models
{
    public class Rental
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
