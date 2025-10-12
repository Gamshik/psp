namespace Videotheque.VideoService.Api.Models.DTO
{
    /// <summary>
    /// DTO для передачи и обработки данных о видео
    /// </summary>
    public class VideoDTO
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = "";
        public string Genre { get; set; } = "";
        public int Year { get; set; }
        public string Director { get; set; } = "";
        public decimal Price { get; set; }
        public int AvailableCopies { get; set; }
    }
}
