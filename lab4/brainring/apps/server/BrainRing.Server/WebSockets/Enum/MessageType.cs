namespace BrainRing.Server.WebSockets.Enum
{
    public enum MessageType
    {
        NewQuestion = 0,
        AnswerResult = 1,
        Error = 2,
        UpdateParticipants = 3,
        CloseGame = 4,
    }
}
