namespace Videotheque.RentalService.Api.Models.DTO
{
    public class VideoDTO
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = "";
        public decimal Price { get; set; }
    }
}
