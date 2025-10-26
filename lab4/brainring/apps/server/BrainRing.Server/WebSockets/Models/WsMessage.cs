using BrainRing.Server.WebSockets.Enum;

namespace BrainRing.Server.WebSockets.Models
{
    public class WsMessage
    {
        public MessageType Type { get; set; }
        public object? Payload { get; set; }
    }
}
