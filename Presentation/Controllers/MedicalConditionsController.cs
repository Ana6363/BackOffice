using Microsoft.AspNetCore.Mvc;


    [ApiController]
    [Route("api/v1/medical-conditions")]
    public class MedicalConditionsController : ControllerBase
    {
        private readonly MedicalConditionsService _medicalConditionsService;
        private readonly ILogger<MedicalConditionsController> _logger;

        public MedicalConditionsController(MedicalConditionsService medicalConditionsService, ILogger<MedicalConditionsController> logger)
        {
            _medicalConditionsService = medicalConditionsService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CreateMedicalCondition([FromBody] MedicalConditionsDto medicalCondition)
        {
            if (medicalCondition == null || string.IsNullOrEmpty(medicalCondition.Name) || string.IsNullOrEmpty(medicalCondition.Description))
            {
                _logger.LogWarning("Invalid medical condition data received.");
                return BadRequest("Invalid medical condition data.");
            }
            try
            {
                var result = await _medicalConditionsService.CreateMedicalConditionAsync(medicalCondition);
                if (result)
                {
                    return Created(string.Empty, new { message = "Medical condition created successfully." });
                }
                else
                {
                    return StatusCode(500, new { message = "Failed to create medical condition." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the medical condition.");
                return StatusCode(500, new { message = "An error occurred while creating the medical condition." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMedicalConditions()
        {
            try
            {
                var medicalConditions = await _medicalConditionsService.GetAllMedicalConditionsAsync();
                if (medicalConditions == null || !medicalConditions.Any())
                {
                    return NotFound(new { message = "No medical conditions found." });
                }
                return Ok(new { message = "All medical conditions fetched successfully.", data = medicalConditions });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching medical conditions.");
                return StatusCode(500, new { message = "An error occurred while fetching medical conditions." });
            }
        }
    }

