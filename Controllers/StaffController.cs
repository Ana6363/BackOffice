using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using BackOffice.Domain.Staff;
using BackOffice.Infrastructure.Staff;
using BackOffice.Application.StaffService;
using Microsoft.Extensions.Configuration;

namespace BackOffice.Controllers
{
    [ApiController]
    [Route("staff")]
    public class StaffController : ControllerBase
    {
        private readonly IStaffRepository _staffRepository;
        private readonly StaffService _staffService;
        private readonly IConfiguration _configuration;

        public StaffController(IStaffRepository staffRepository, StaffService staffService, IConfiguration configuration)
        {
            _staffRepository = staffRepository;
            _staffService = staffService;
            _configuration = configuration;
        }

        [HttpPost("create")]
            public async Task<IActionResult> CreateStaffAsync([FromBody] StaffDto staffDto)
            {
                if (staffDto == null)
                {
                    return BadRequest(new { success = false, message = "Staff details are required." });
                }

                try
                {
                    var staffDataModel = await _staffService.CreateStaffAsync(staffDto, _configuration);
                    return Ok(new { success = true, staff = staffDataModel });
                }
                catch (Exception ex)
                {
                    return BadRequest(new { success = false, message = ex.Message });
                }
            }
    }
}
