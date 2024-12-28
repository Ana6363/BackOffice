using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

public class PatientMedicalRecordService{

    private readonly HttpClient _httpClient;
    private readonly ILogger<PatientMedicalRecordService> _logger;
    private readonly string _nodeJsBackendUrl;

    public PatientMedicalRecordService(HttpClient httpClient, ILogger<PatientMedicalRecordService> logger, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _logger = logger;
        _nodeJsBackendUrl = configuration["NodeJsBackendUrl"];
    }

    public async Task<bool> UpdatePatientMedicalRecordAsync(PatientMedicalRecordDto patientMedicalRecordDto)
    {
        try
        {
            var url = $"{_nodeJsBackendUrl}/patient-medical-records";

            var json = JsonSerializer.Serialize(patientMedicalRecordDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _logger.LogInformation("Sending POST request to Node.js backend. URL: {Url}, Body: {Body}", url, json);

            var response = await _httpClient.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Patient medical record created successfully in Node.js backend.");
                return true;
            }
            else
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Failed to create patient medical record. Status: {StatusCode}, Response: {ResponseBody}",
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

    public async Task<IEnumerable<PatientMedicalRecordDto>> GetAllPatientMedicalRecordsAsync()
    {
        try
        {
            var url = $"{_nodeJsBackendUrl}/patient-medical-records";

            _logger.LogInformation("Sending GET request to Node.js backend. URL: {Url}", url);

            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Patient medical records fetched successfully from Node.js backend. Response: {ResponseBody}", responseBody);

                var jsonResponse = JsonSerializer.Deserialize<JsonElement>(responseBody);

                if(jsonResponse.TryGetProperty("data", out var patientMedicalRecordsJson))
                {
                    var patientMedicalRecords = JsonSerializer.Deserialize<IEnumerable<PatientMedicalRecordDto>>(patientMedicalRecordsJson.GetRawText(), new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    
                    return patientMedicalRecords;
                }
                 throw new JsonException("Failed to parse patient medical records from JSON response.");

                
            }
            else
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Failed to get patient medical records. Status: {StatusCode}, Response: {ResponseBody}",
                    response.StatusCode, responseBody);
                
                throw new HttpRequestException($"Failed to get patient medical records: {response.StatusCode} - {responseBody}");

            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while sending GET request to Node.js backend.");
            throw;
        }
        
    }    
}