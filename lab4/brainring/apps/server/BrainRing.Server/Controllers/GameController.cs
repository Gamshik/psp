using BrainRing.Application.Interfaces.Services;
using BrainRing.Application.Params.GameSession;
using BrainRing.Server.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace BrainRing.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GameController : ControllerBase
    {
        private readonly IGameSessionService _gameSessionService;

        public GameController(IGameSessionService gameSessionService)
        {
            _gameSessionService = gameSessionService;
        }

        [HttpPost("create")]
        public async Task<ActionResult<GameSessionDto>> Create()
        {
            if (!Request.Cookies.TryGetValue("userId", out var userIdStr) ||
                !Guid.TryParse(userIdStr, out var userId))
                return Unauthorized();

            var session = await _gameSessionService.CreateGameSessionAsync(
                new CreateGameSessionParams { HostId = userId });

            return Ok(new GameSessionDto
            {
                Id = session.Id,
                HostId = session.HostId,
                IsActive = session.IsActive
            });
        }

        [HttpGet("check")]
        public async Task<ActionResult<GameSessionDto>> Check([FromQuery] Guid sessionId)
        {
            var session = await _gameSessionService.GetGameSessionAsync(sessionId);
            if (session == null || !session.IsActive)
                return NotFound("Комната не найдена или не активна");

            return Ok(new GameSessionDto
            {
                Id = session.Id,
                HostId = session.HostId,
                IsActive = session.IsActive
            });
        }
    }
}
