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
        _nodeJsBackendUrl = configuration["NodeJsBackend:BaseUrl"];
    }

   public async Task<bool> UpdatePatientMedicalRecordAsync(PatientMedicalRecordDto patientMedicalRecordDto)
{
    try
    {
        var url = $"{_nodeJsBackendUrl}/patient-medical-records";

        // Send a flat payload instead of wrapping it in "records"
        var payload = new
        {
            recordNumber = patientMedicalRecordDto.RecordNumber,
            allergies = patientMedicalRecordDto.Allergies,
            medicalConditions = patientMedicalRecordDto.MedicalConditions
        };

        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        _logger.LogInformation("Sending PUT request to Node.js backend. URL: {Url}, Body: {Body}", url, json);

        var response = await _httpClient.PutAsync(url, content);

        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation("Patient medical record updated successfully in Node.js backend.");
            return true;
        }
        else
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            _logger.LogWarning("Failed to update patient medical record. Status: {StatusCode}, Response: {ResponseBody}",
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

            // Log the raw JSON response for debugging
            _logger.LogInformation("Raw Response Body: {ResponseBody}", responseBody);

            var jsonResponse = JsonSerializer.Deserialize<JsonElement>(responseBody);
            _logger.LogInformation("Parsed JSON Response: {JsonResponse}", jsonResponse);

            if (jsonResponse.TryGetProperty("data", out var patientMedicalRecordsJson))
            {
                _logger.LogInformation("Parsed Patient Medical Records Data: {PatientMedicalRecordsJson}", patientMedicalRecordsJson);

                // Deserialize the patient medical records
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
        _logger.LogError(ex, "An error occurred while fetching patient medical records.");
        throw;
    }
}



    public async Task<bool> SyncRecordsToNodeJsAsync(IEnumerable<object> records)
{
    try
    {
        var url = $"{_nodeJsBackendUrl}/patient-medical-records";
        Console.WriteLine("NFJKNSDJKFNJDSKF");
        Console.WriteLine(url);

        // Serialize the records into JSON
        var json = JsonSerializer.Serialize(new { records });
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        _logger.LogInformation("Sending POST request to Node.js backend to sync patient records. URL: {Url}, Body: {Body}", url, json);

        // Send POST request
        var response = await _httpClient.PostAsync(url, content);

        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation("Patient records synced successfully to the Node.js backend.");
            return true;
        }
        else
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            _logger.LogWarning("Failed to sync patient records. Status: {StatusCode}, Response: {ResponseBody}",
                response.StatusCode, responseBody);
            return false;
        }
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "An error occurred while syncing patient records to the Node.js backend.");
        throw;
    }
}
 public async Task<byte[]> GetPatientMedicalRecordForDownloadAsync(string recordNumber)
{
    try
    {
        var url = $"{_nodeJsBackendUrl}/patient-medical-records/{recordNumber}";
        _logger.LogInformation("Sending GET request to Node.js backend. URL: {Url}", url);
        var response = await _httpClient.GetAsync(url);
        if (response.IsSuccessStatusCode)
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Patient medical record fetched successfully from Node.js backend. Response: {ResponseBody}", responseBody);
            var jsonResponse = JsonSerializer.Deserialize<JsonElement>(responseBody);
            if (jsonResponse.TryGetProperty("data", out var patientMedicalRecordJson))
            {
                // Converter para JSON e retornar como bytes para o download
                var jsonContent = patientMedicalRecordJson.GetRawText();
                return Encoding.UTF8.GetBytes(jsonContent);  // Retorna os dados como um arquivo JSON (bytes)
            }
            throw new JsonException("Failed to parse patient medical record from JSON response.");
        }
        else
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            _logger.LogWarning("Failed to get patient medical record. Status: {StatusCode}, Response: {ResponseBody}",
                response.StatusCode, responseBody);
            throw new HttpRequestException($"Failed to get patient medical record: {response.StatusCode} - {responseBody}");
        }
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "An error occurred while sending GET request to Node.js backend.");
        throw;
    }
}
    
}