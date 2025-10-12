namespace Videotheque.VideoService.Api.Models.Domain
{
    /// <summary>
    /// Сущность видеозаписи (фильма)
    /// </summary>
    public class Video
    {
        /// <summary>
        /// Уникальный идентификатор фильма
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Название фильма
        /// </summary>
        public string Title { get; set; } = "";

        /// <summary>
        /// Жанр
        /// </summary>
        public string Genre { get; set; } = "";

        /// <summary>
        /// Год выпуска
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// Режиссер
        /// </summary>
        public string Director { get; set; } = "";

        /// <summary>
        /// Стоимость аренды или покупки
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Количество доступных копий
        /// </summary>
        public int AvailableCopies { get; set; }
    }
}
