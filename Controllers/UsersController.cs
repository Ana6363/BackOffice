using BackOffice.Domain.Users; 
using Microsoft.AspNetCore.Mvc;

namespace BackOffice.Controllers
{
    public class UsersController : Controller
    {
        private readonly ILogger<UsersController> _logger;
        private readonly UserService _userService;

        public UsersController(ILogger<UsersController> logger, UserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        [HttpGet("users")]
        public async Task<IActionResult> Get()
        {
            var res = await _userService.GetAllAsync();
            return Ok(res);
        }

        [HttpPost("users")]
        public async Task<IActionResult> Post(UserDto user)
        {
            var res = await _userService.AddAsync(user);
            if (res == null)
            {
                return BadRequest();
            }
            return Ok();
        }

        [HttpDelete("users")]
        public async Task<IActionResult> Delete(UserId id)
        {
            var res = await _userService.DeleteAsync(id);
            if (res == null)
            {
                return BadRequest();
            }
            return Ok();
        }
    }
}
