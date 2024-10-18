using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using BackOffice.Application.Patients;
using BackOffice.Domain.Patients;

namespace BackOffice.Controllers
{
    [ApiController]
    [Route("patient")]
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


        [HttpGet] 
        public async Task<IActionResult> GetAllPatientsAsync()
        {
            try
            {
                var patients = await _patientRepository.GetAllAsync(); // Get all patients
                return Ok(new { success = true, patients });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}
