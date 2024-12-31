using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

public class AllergyService
{
    private readonly HttpClient _httpClient;
    private readonly string _nodeJsBackendUrl;
    private readonly ILogger<AllergyService> _logger;

    public AllergyService(HttpClient httpClient, IConfiguration configuration, ILogger<AllergyService> logger)
    {
        _httpClient = httpClient;
        _nodeJsBackendUrl = configuration["NodeJsBackend:BaseUrl"]; // Configured Node.js Base URL
        _logger = logger;
    }

    public async Task<bool> CreateAllergyAsync(AllergyDto allergy)
    {
        try
        {
            var url = $"{_nodeJsBackendUrl}/allergies"; // Construct the full URL
            var payload = JsonSerializer.Serialize(allergy); // Serialize the AllergyDto to JSON
            var jsonContent = new StringContent(payload, Encoding.UTF8, "application/json"); // Prepare HTTP content

            _logger.LogInformation("Sending POST request to Node.js backend. URL: {Url}, Payload: {Payload}", url, payload);

            var response = await _httpClient.PostAsync(url, jsonContent); // Send the POST request

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Allergy created successfully via Node.js backend.");
                return true;
            }
            else
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Failed to create allergy. Status: {StatusCode}, Response: {ResponseBody}",
                    response.StatusCode, responseBody);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while sending POST request to Node.js backend.");
            throw;
        }
    }

    public async Task<IEnumerable<AllergyDto>> GetAllAllergiesAsync()
        {
            try
            {
                var url = $"{_nodeJsBackendUrl}/allergies";
                _logger.LogInformation("Sending GET request to Node.js backend. URL: {Url}", url);

                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation("Allergies fetched successfully from Node.js backend. Response: {ResponseBody}", responseBody);

                    // Deserialize the response into an intermediate structure
                    var jsonResponse = JsonSerializer.Deserialize<JsonElement>(responseBody);

                    // Extract the "data" property
                    if (jsonResponse.TryGetProperty("data", out var allergiesJson))
                    {
                        var allergies = JsonSerializer.Deserialize<IEnumerable<AllergyDto>>(allergiesJson.GetRawText(), new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                        return allergies;
                    }

                    throw new JsonException("The 'data' property was not found in the response.");
                }
                else
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("Failed to fetch allergies. Status: {StatusCode}, Response: {ResponseBody}",
                        response.StatusCode, responseBody);

                    throw new HttpRequestException($"Failed to fetch allergies: {response.StatusCode} - {responseBody}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while sending GET request to Node.js backend.");
                throw;
            }
        }

        public async Task<bool> UpdateAllergyAsync(AllergyDto allergy)
    {
        try
        {
            var url = $"{_nodeJsBackendUrl}/allergies"; // Construct the full URL
            var payload = JsonSerializer.Serialize(allergy); // Serialize the AllergyDto to JSON
            var jsonContent = new StringContent(payload, Encoding.UTF8, "application/json"); // Prepare HTTP content

            _logger.LogInformation("Sending PUT request to Node.js backend. URL: {Url}, Payload: {Payload}", url, payload);

            var response = await _httpClient.PutAsync(url, jsonContent); // Send the POST request

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Allergy updated successfully via Node.js backend.");
                return true;
            }
            else
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Failed to update allergy. Status: {StatusCode}, Response: {ResponseBody}",
                    response.StatusCode, responseBody);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while sending PUT request to Node.js backend.");
            throw;
        }
    }

}
