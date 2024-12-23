using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/v1/allergies")]
public class AllergyController : ControllerBase
{
    private readonly AllergyService _allergyService;
    private readonly ServiceBusService _serviceBusService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AllergyController> _logger;

    public AllergyController(AllergyService allergyService, ServiceBusService serviceBusService,
        IConfiguration configuration, ILogger<AllergyController> logger)
    {
        _allergyService = allergyService;
        _serviceBusService = serviceBusService;
        _configuration = configuration;
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

        _logger.LogInformation("Sending allergy to Service Bus: {Allergy}", allergy);

        var messageBody = new
        {
            type = "allergy",
            data = allergy,
        };

        await _serviceBusService.SendMessageAsync(_configuration["ServiceBus:QueueName"], messageBody);
        _logger.LogInformation("Message sent to Service Bus.");

        // Initialize TaskCompletionSource
        _serviceBusService.InitializeFeedbackTask();

        // Wait for feedback
        var feedback = await _serviceBusService.WaitForFeedbackAsync();

        if (feedback == null)
        {
            return StatusCode(500, "No feedback received.");
        }

        _logger.LogInformation("Feedback received: StatusCode={StatusCode}, Message={Message}", feedback.StatusCode, feedback.Message);

        // Return the feedback status code and message as the response
        return StatusCode(feedback.StatusCode, feedback.Message);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "An error occurred while sending allergy creation request to Service Bus.");
        return StatusCode(500, "An error occurred while sending the request.");
    }
}
}