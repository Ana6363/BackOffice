using BackOffice.Application.Appointement;
using BackOffice.Domain.Appointement;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackOffice.Controllers
{
    [ApiController]
    [Route("api/v1/appointement")]
    public class AppointementController : ControllerBase
    {
        private readonly AppointementService _appointementService;
        private readonly IAppointementRepository _appointementRepository;

        public AppointementController(AppointementService appointementService, IAppointementRepository appointementRepository)
        {
            _appointementService = appointementService;
            _appointementRepository = appointementRepository;
        }


      [HttpPost("create")]
     // [Authorize(Roles ="Doctor")]
        public async Task<IActionResult> CreateAppointementAsync([FromBody] AppointementDto appointement)
        {
            if (appointement == null)
            {
                return BadRequest(new { success = false, message = "Appointement details are required." });
            }

            try
            {
                var appointementDataModel = await _appointementService.CreateAppointementAsync(appointement);
                return Ok(new { success = true, appointement = appointementDataModel });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        } 


      /*  [HttpPut("update")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> UpdateAppointementAsync([FromBody] AppointementDto appointement)
        {
            if (appointement == null)
            {
                return BadRequest(new { success = false, message = "Appointement details are required." });
            }

            try
            {
                var appointementDataModel = await _appointementService.UpdateAppointementAsync(appointement);
                return Ok(new { success = true, appointement = appointementDataModel });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        } */


        [HttpDelete("delete")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> DeleteAppointementAsync([FromBody] AppointementId appointement)
        {
            if (appointement == null)
            {
                return BadRequest(new { success = false, message = "Appointement details are required." });
            }

            try
            {
                var appointementDataModel = await _appointementService.DeleteAppointementAsync(appointement);
                return Ok(new { success = true, appointement = appointementDataModel });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }


        [HttpGet("getAll")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> GetAllAppointementsAsync()
        {
            try
            {
                var appointements = await _appointementService.GetAppointementsAsync();
                return Ok(new { success = true, appointements });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }


        [HttpGet("getById")]
        [Authorize(Roles = "Doctor")]
        public async Task<IActionResult> GetAppointementByIdAsync([FromBody] AppointementId id)
        {
            if (id == null)
            {
                return BadRequest(new { success = false, message = "Appointement ID is required." });
            }

            try
            {
                var appointement = await _appointementService.GetAppointementByIdAsync(id);
                return Ok(new { success = true, appointement });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}
