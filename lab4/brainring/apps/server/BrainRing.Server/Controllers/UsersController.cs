using BrainRing.Application.Interfaces.Services;
using BrainRing.Application.Params.User;
using BrainRing.Server.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace BrainRing.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register([FromBody] RegisterUserDto dto)
        {
            var getParams = new GetUserByNameParams
            {
                Name = dto.Name,
            };

            var existing = await _userService.GetUserByName(getParams);
            if (existing != null)
                return Conflict("User with this name already exists.");

            var createParams = new CreateUserParams
            {
                Name = dto.Name
            };

            var user = await _userService.CreateUserAsync(createParams);
            return new UserDto { Id = user.Id, Name = user.Name };
        }
    }
}
