using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

public class AllergyService
{
    private readonly HttpClient _httpClient;
    private readonly string _nodeJsBackendUrl;
    
    private readonly ILogger<AllergyController> _logger;
    public AllergyService(HttpClient httpClient, IConfiguration configuration,ILogger<AllergyController> logger)
    {
        _httpClient = httpClient;
        _nodeJsBackendUrl = configuration["NodeJsBackend:BaseUrl"]; // Configured URL for Node.js backend
        _logger = logger;
    }

   public async Task<bool> CreateAllergyAsync(AllergyDto allergy)
{
    try
    {
        var url = $"{_nodeJsBackendUrl}/allergies";
        var payload = JsonSerializer.Serialize(allergy);
        var jsonContent = new StringContent(payload, Encoding.UTF8, "application/json");

        _logger.LogInformation("Sending POST request to Node.js backend. URL: {Url}, Payload: {Payload}", url, payload);

        var response = await _httpClient.PostAsync(url, jsonContent);

        var responseBody = await response.Content.ReadAsStringAsync();
        _logger.LogInformation("Received response from Node.js backend. Status: {StatusCode}, Body: {ResponseBody}",
            response.StatusCode, responseBody);

        return response.IsSuccessStatusCode;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "An error occurred while sending POST request to Node.js backend.");
        throw;
    }
}



    public async Task<IEnumerable<AllergyDto>> GetAllAllergiesAsync()
        {
            // Send GET request to Node.js backend
            var response = await _httpClient.GetAsync($"{_nodeJsBackendUrl}/allergies");

            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"Failed to fetch allergies: {response.StatusCode}");

            // Deserialize the JSON response
            var json = await response.Content.ReadAsStringAsync();

            // Deserialize only the relevant part of the JSON (the 'data' field)
            var allergyResponse = JsonSerializer.Deserialize<JsonElement>(json);

            // Check if the 'data' field exists and contains a valid array
            if (allergyResponse.TryGetProperty("data", out var data))
            {
                var allergies = new List<AllergyDto>();

                foreach (var item in data.EnumerateArray())
                {
                    var allergy = new AllergyDto
                    {
                        Name = item.GetProperty("name").GetString(),
                        Description = item.GetProperty("description").GetString()
                    };

                    allergies.Add(allergy);
                }

                return allergies;
            }

            throw new JsonException("Failed to find 'data' property in the response.");
        }

}
