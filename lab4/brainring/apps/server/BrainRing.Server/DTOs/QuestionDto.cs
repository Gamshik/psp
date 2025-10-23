namespace BrainRing.Server.DTOs
{
    public class QuestionDto
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
        public List<QuestionOptionDto> Options { get; set; } = new();
    }
}
