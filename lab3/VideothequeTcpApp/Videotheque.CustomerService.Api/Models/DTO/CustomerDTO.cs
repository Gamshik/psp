namespace Videotheque.CustomerService.Api.Models.DTO
{
    public class CustomerDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string PhoneNumber { get; set; } = "";
        public DateTime RegisteredAt { get; set; }
    }
}
