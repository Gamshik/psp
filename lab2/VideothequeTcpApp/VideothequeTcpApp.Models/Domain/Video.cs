namespace VideothequeTcpApp.Models.Domain
{
    /// <summary>
    /// Модель данных для видеозаписи.
    /// Используется как в клиенте, так и на сервере.
    /// </summary>
    public class Video
    {
        /// <summary>
        /// Уникальный идентификатор видео.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Название видео.
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Жанр видео (например, Action, Sci-Fi).
        /// </summary>
        public string Genre { get; set; } = string.Empty;

        /// <summary>
        /// Год выпуска видео.
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// Режиссёр видео.
        /// </summary>
        public string Director { get; set; } = string.Empty;

        /// <summary>
        /// Цена видео (например, аренда или покупка).
        /// Используется decimal для точного представления денежных значений.
        /// </summary>
        public decimal Price { get; set; }
    }
}
