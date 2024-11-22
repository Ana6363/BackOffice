using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using BackOffice.Application.Patients;
using BackOffice.Domain.Patients;
using BackOffice.Infrastructure.Patients;

namespace BackOffice.Controllers
{
    [ApiController]
    [Route("api/v1/patient")]
    public class PatientController : ControllerBase
    {
        private readonly IPatientRepository _patientRepository;
        private readonly PatientService _patientService;

        public PatientController(IPatientRepository patientRepository, PatientService patientService)
        {
            _patientRepository = patientRepository;
            _patientService = patientService;
        }

        [HttpPost("create")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreatePatientAsync([FromBody] PatientDto patientDto)
        {
            if (patientDto == null)
            {
                return BadRequest(new { success = false, message = "Patient details are required." });
            }

            try
            {
                // Use the PatientService to create the patient
                var patientDataModel = await _patientService.CreatePatientAsync(patientDto);
                return Ok(new { success = true, patient = patientDataModel });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("filter")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllPatientsAsync(
            [FromQuery] string? userId = null,
            [FromQuery] int? phoneNumber = null,
            [FromQuery] string? firstName = null,
            [FromQuery] string? lastName = null,
            [FromQuery] string? fullName = null,
            [FromQuery] bool? isToBeDeleted = null,
            [FromQuery] string? recordNumber= null
            )
        {
            try
            {
                // Create a filter DTO from the query parameters
                var filterDto = new PatientFilterDto(userId,phoneNumber, firstName, lastName, fullName,isToBeDeleted,recordNumber);

                var patients = await _patientService.GetFilteredPatientsAsync(filterDto); // Filter based on dto
                return Ok(new { success = true, patients });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPut("update")]
        [Authorize(Roles = "Admin, Patient")]
        public async Task<IActionResult> UpdatePatientAsync([FromBody] PatientDto patientDto)
        {
            if (patientDto == null || string.IsNullOrWhiteSpace(patientDto.RecordNumber))
            {
                return BadRequest(new { success = false, message = "Details including RecordNumber are required." });
            }

            try
            {
                var updatedPatient = await _patientService.UpdateAsync(patientDto);
                return Ok(new { success = true, patient = updatedPatient });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpPut("markToDelete")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> MarkPatientToDeleteAsync([FromBody] RecordNumberDto recordNumberDto)
        {
            try
            {
                var recordNumObj = new RecordNumber(recordNumberDto.RecordNumber);

                await _patientService.MarkToDelete(recordNumObj);
                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }


        [HttpDelete("delete/{recordNumber}")]
        [Authorize(Roles = "Admin, Patient")]
        public async Task<IActionResult> DeletePatientAsync([FromRoute] string recordNumber)
        {
            if (string.IsNullOrWhiteSpace(recordNumber))
            {
                return BadRequest(new { success = false, message = "Patient details are required." });
            }

            try
            {
                // Convert the string recordNumber to the RecordNumber domain object
                var recordNumObj = new RecordNumber(recordNumber);

                // Use the PatientService to delete the patient
                var patientDataModel = await _patientService.DeletePatientAsync(recordNumObj);
                return Ok(new { success = true, patient = patientDataModel });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("loggedPatient")]
        [Authorize(Roles = "Patient")]
        public async Task<IActionResult> GetLoggedPatientAsync()
        {
            try
            {
                var patients = await _patientService.GetLoggedPatientAsync();
                return Ok(new { success = true, patients });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}
