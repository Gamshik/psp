namespace BrainRing.Server.DTOs
{
    public class GameSessionDto
    {
        public Guid Id { get; set; }
        public Guid HostId { get; set; }
        public List<UserDto> Participants { get; set; } = new();
        public QuestionDto? CurrentQuestion { get; set; }
        public bool IsActive { get; set; }
    }
}
