using BrainRing.Application.Interfaces.Services;
using BrainRing.Application.Params.User;
using BrainRing.Domain.Entities;
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

            var existing = _userService.GetUserByName(getParams);
            if (existing != null)
                return Conflict("User with this name already exists.");

            var createParams = new CreateUserParams
            {
                Name = dto.Name
            };

            var user = await _userService.CreateUserAsync(createParams);

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true, 
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            };

            Response.Cookies.Append("userId", user.Id.ToString(), cookieOptions);

            return Ok(new UserDto{ Id = user.Id, Name = user.Name });
        }

        [HttpPost("login")]
        public ActionResult<UserDto> Login([FromBody] GetUserByNameParams @params)
        {
            var user = _userService.GetUserByName(@params);
            if (user == null)
                return Unauthorized(new { message = "User not found" });

            Response.Cookies.Append("userId", user.Id.ToString(), new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.Strict,
                Secure = false,
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            });

            return Ok(new UserDto{ Id = user.Id, Name = user.Name });
        }

        [HttpGet("auth/check")]
        public ActionResult<UserDto> CheckAuth()
        {
            if (HttpContext.Items["User"] is not User user)
                return Unauthorized();

            return Ok(new UserDto { Id = user.Id, Name = user.Name });
        }
    }
}
