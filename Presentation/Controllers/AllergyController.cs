using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

[ApiController]
[Route("api/v1/allergies")]
public class AllergyController : ControllerBase
{
    private readonly AllergyService _allergyService;
    private readonly ILogger<AllergyController> _logger;

    public AllergyController(AllergyService allergyService, ILogger<AllergyController> logger)
    {
        _allergyService = allergyService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> CreateAllergy([FromBody] AllergyDto allergy)
    {
        if (allergy == null || string.IsNullOrEmpty(allergy.Name) || string.IsNullOrEmpty(allergy.Description))
        {
            _logger.LogWarning("Invalid allergy data received.");
            return BadRequest("Invalid allergy data.");
        }

        try
        {
            var result = await _allergyService.CreateAllergyAsync(allergy);

            if (result)
            {
                return Created(string.Empty, new { message = "Allergy created successfully." });
            }
            else
            {
                return StatusCode(500, new { message = "Failed to create allergy." });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while creating the allergy.");
            return StatusCode(500, new { message = "An error occurred while creating the allergy." });
        }
    }

        [HttpGet]
        public async Task<IActionResult> GetAllAllergies()
        {
            try
            {
                var allergies = await _allergyService.GetAllAllergiesAsync();

                if (allergies == null || !allergies.Any())
                {
                    return NotFound(new { message = "No allergies found." });
                }

                return Ok(new { message = "All allergies fetched successfully.", data = allergies });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching allergies.");
                return StatusCode(500, new { error = "An error occurred while fetching allergies." });
            }
        }
}
