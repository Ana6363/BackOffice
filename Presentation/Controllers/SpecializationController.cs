using BackOffice.Application.Specialization;
using BackOffice.Domain.Specialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace BackOffice.Presentation.Controllers
{
    [ApiController]
    [Route("api/v1/specialization")]
    public class SpecializationController : ControllerBase // Inherit from ControllerBase
    {
        private readonly ISpecializationRepository _specializationRepository;
        private readonly SpecializationService _specializationService;

        public SpecializationController(ISpecializationRepository specializationRepository, SpecializationService specializationService)
        {
            _specializationRepository = specializationRepository;
            _specializationService = specializationService;
        }

        [HttpPost("create")]
       // [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateSpecializationAsync([FromBody] SpecializationDto specializationDto)
        {
            if (specializationDto == null)
            {
                return BadRequest(new { success = false, message = "Specialization details are required." });
            }
            try
            {
                var specializationDataModel = await _specializationService.AddAsync(specializationDto);
                return Ok(new { success = true, specialization = specializationDataModel });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("filter")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllSpecializationsAsync(
            [FromQuery] string? id = null,
            [FromQuery] string? description = null)
        {
            try
            {
                var filter = new SpecializationFilterDto(id, description);
                var specializations = await _specializationService.GetFilteredAsync(filter);
                return Ok(new { success = true, specializations });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPut("update")]
       // [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateSpecializationAsync([FromBody] SpecializationDto specializationDto)
        {
            if (specializationDto == null)
            {
                return BadRequest(new { success = false, message = "Specialization details are required." });
            }
            try
            {
                var specializationDataModel = await _specializationService.UpdateAsync(specializationDto);
                return Ok(new { success = true, specialization = specializationDataModel });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpDelete("delete")]
       // [Authorize(Roles ="Admin")]
        public async Task<IActionResult> DeleteSpecializationAsync([FromBody] SpecializationDto dto)
        {
            if (dto == null)
            {
                return BadRequest(new { success = false, message = "Specialization is required." });
            }
            try
            {
                await _specializationService.DeleteAsync(dto);
                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

    }
}
