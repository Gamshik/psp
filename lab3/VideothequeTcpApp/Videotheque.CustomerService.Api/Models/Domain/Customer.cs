namespace Videotheque.CustomerService.Api.Models.Domain
{
    /// <summary>
    /// Сущность клиента видеотеки
    /// </summary>
    public class Customer
    {
        /// <summary>
        /// Уникальный ID клиента
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Имя
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// Электронная почта
        /// </summary>
        public string Email { get; set; } = "";

        /// <summary>
        /// Телефон
        /// </summary>
        public string PhoneNumber { get; set; } = "";

        /// <summary>
        /// Дата регистрации
        /// </summary>
        public DateTime RegisteredAt { get; set; }
    }
}
