using BackOffice.Domain.Users; 
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackOffice.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class UsersController : Controller
    {
        private readonly ILogger<UsersController> _logger;
        private readonly UserService _userService;

        public UsersController(ILogger<UsersController> logger, UserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var res = await _userService.GetAllAsync();
            return Ok(res);
        }

        [HttpPost]
        public async Task<IActionResult> Post(UserDto user)
        {
            var res = await _userService.AddAsync(user);
            if (res == null)
            {
                return BadRequest();
            }
            return Ok();
        }

        [HttpDelete]
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
