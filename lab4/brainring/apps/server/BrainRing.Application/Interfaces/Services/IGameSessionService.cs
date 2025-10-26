using BrainRing.Application.Models.GameSession;
using BrainRing.Application.Params.GameSession;
using BrainRing.Domain.Entities;

namespace BrainRing.Application.Interfaces.Services
{
    public interface IGameSessionService
    {
        Task<GameSessionResult> CreateGameSessionAsync(CreateGameSessionParams @params, CancellationToken token = default);
        Task<GameSessionResult> JoinGameSessionAsync(JoinGameSessionParams @params, CancellationToken token = default);
        Task<GameSessionResult> LeaveGameSessionAsync(LeaveGameSessionParams @params, CancellationToken token = default);
        Task<GameSessionResult> CloseGameSessionAsync(CloseGameSessionParams @params, CancellationToken token = default);
        Task<GameSessionResult?> GetGameSessionAsync(Guid sessionId, CancellationToken token = default);
    }
}
