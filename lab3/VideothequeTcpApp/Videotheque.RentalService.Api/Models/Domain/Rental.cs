namespace Videotheque.RentalService.Api.Models.Domain
{
    /// <summary>
    /// Сущность аренды видеофильма клиентом
    /// </summary>
    public class Rental
    {
        /// <summary>
        /// Уникальный ID аренды
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// ID клиента
        /// </summary>
        public Guid CustomerId { get; set; }

        /// <summary>
        /// ID фильма
        /// </summary>
        public Guid VideoId { get; set; }

        /// <summary>
        /// Дата начала аренды
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Дата окончания аренды
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Возвращен ли фильм
        /// </summary>
        public bool IsReturned { get; set; }

        /// <summary>
        /// Итоговая стоимость
        /// </summary>
        public decimal TotalPrice { get; set; }
    }
}
