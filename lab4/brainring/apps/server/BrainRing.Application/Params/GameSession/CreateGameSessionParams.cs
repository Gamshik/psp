namespace BrainRing.Application.Params.GameSession
{
    public class CreateGameSessionParams
    {
        public Guid HostId { get; set; }
        public List<Guid> ParticipantIds { get; set; } = new();
    }
}
