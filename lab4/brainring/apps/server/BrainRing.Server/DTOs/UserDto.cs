namespace BrainRing.Server.DTOs
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Score { get; set; } = 0;
    }
}
