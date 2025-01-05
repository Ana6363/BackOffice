using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

    public class MedicalConditionsService
    {
        private readonly HttpClient _httpClient;
        private readonly string _nodeJsBackendUrl;
        private readonly ILogger<MedicalConditionsService> _logger;

        public MedicalConditionsService(HttpClient httpClient, IConfiguration configuration, ILogger<MedicalConditionsService> logger)
        {
            _httpClient = httpClient;
            _nodeJsBackendUrl = configuration["NodeJsBackend:BaseUrl"]; // Configured Node.js Base URL
            _logger = logger;
        }

        public async Task<bool> CreateMedicalConditionAsync(MedicalConditionsDto medicalCondition)
        {
            try
            {
                var url = $"{_nodeJsBackendUrl}/medical-conditions"; // Construct the full URL
                var payload = JsonSerializer.Serialize(medicalCondition); // Serialize the MedicalConditionDto to JSON
                var jsonContent = new StringContent(payload, Encoding.UTF8, "application/json"); // Prepare HTTP content

                _logger.LogInformation("Sending POST request to Node.js backend. URL: {Url}, Payload: {Payload}", url, payload);

                var response = await _httpClient.PostAsync(url, jsonContent); // Send the POST request
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Medical condition created successfully via Node.js backend.");
                    return true;
                }
                else
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("Failed to create medical condition. Status: {StatusCode}, Response: {ResponseBody}",
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

        public async Task<IEnumerable<MedicalConditionsDto>> GetAllMedicalConditionsAsync()
        {
            try
            {
                var url = $"{_nodeJsBackendUrl}/medical-conditions";

                _logger.LogInformation("Sending GET request to Node.js backend. URL: {Url}", url);

                var response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation("Medical conditions fetched successfully from Node.js backend. Response: {ResponseBody}", responseBody);

                    var jsonResponse = JsonSerializer.Deserialize<JsonElement>(responseBody);

                    if(jsonResponse.TryGetProperty("data", out var medicalConditionsJson))
                    {
                        var medicalConditions = JsonSerializer.Deserialize<IEnumerable<MedicalConditionsDto>>(medicalConditionsJson.GetRawText(), new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });
                        
                        return medicalConditions;
                    }
                    
                    throw new JsonException("Failed to parse medical conditions from JSON response.");
                }
                else
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("Failed to get medical conditions. Status: {StatusCode}, Response: {ResponseBody}",
                        response.StatusCode, responseBody);

                    throw new HttpRequestException($"Failed to get medical conditions: {response.StatusCode} - {responseBody}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while sending GET request to Node.js backend.");
                throw;
            }
        }

        public async Task<bool> DeleteMedicalConditionAsync(string name)
    {
        try
        {
            var url = $"{_nodeJsBackendUrl}/medical-conditions/{name}";
            _logger.LogInformation("Sending DELETE request to Node.js backend. URL: {Url}", url);
            var response = await _httpClient.DeleteAsync(url);
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Medical condition deleted successfully via Node.js backend.");
                return true;
            }
            else
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Failed to delete medical condition. Status: {StatusCode}, Response: {ResponseBody}",
                    response.StatusCode, responseBody);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while sending DELETE request to Node.js backend.");
            throw;
        }
    }

    public async Task<bool> UpdateMedicalConditionAsync(MedicalConditionsDto medicalCondition)
    {
        try
        {
            var url = $"{_nodeJsBackendUrl}/medical-conditions";
            var payload = JsonSerializer.Serialize(medicalCondition);
            var jsonContent = new StringContent(payload, Encoding.UTF8, "application/json");
            _logger.LogInformation("Sending PUT request to Node.js backend. URL: {Url}, Payload: {Payload}", url, payload);
            var response = await _httpClient.PutAsync(url, jsonContent);
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Medical condition updated successfully via Node.js backend.");
                return true;
            }
            else
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Failed to update medical condition. Status: {StatusCode}, Response: {ResponseBody}",
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

