using BackOffice.Application.Patients;
using BackOffice.Controllers;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/v1/patient-medical-record")]
public class PatientMedicalRecordController : ControllerBase
{
    private readonly PatientMedicalRecordService _patientMedicalRecordService;
    private readonly ILogger<PatientMedicalRecordController> _logger;
    private readonly PatientService _patientService;

    public PatientMedicalRecordController(PatientMedicalRecordService patientMedicalRecordService, ILogger<PatientMedicalRecordController> logger, PatientService patientService)
    {
        _patientMedicalRecordService = patientMedicalRecordService;
        _logger = logger;
        _patientService = patientService;
    }

    [HttpPut]
public async Task<IActionResult> UpdatePatientMedicalRecord([FromBody] PatientMedicalRecordDto patientMedicalRecord)
{
    // Validate input
    if (patientMedicalRecord == null ||
        string.IsNullOrEmpty(patientMedicalRecord.RecordNumber) ||
        patientMedicalRecord.Allergies == null || !patientMedicalRecord.Allergies.Any() ||
        patientMedicalRecord.MedicalConditions == null || !patientMedicalRecord.MedicalConditions.Any())
    {
        _logger.LogWarning("Invalid patient medical record data received.");
        return BadRequest("Invalid patient medical record data. Ensure all required fields are provided.");
    }

    try
    {
        // Call the service to update the patient medical record
        var result = await _patientMedicalRecordService.UpdatePatientMedicalRecordAsync(patientMedicalRecord);
        
        if (result)
        {
            return Ok(new { message = "Patient medical record updated successfully." });
        }
        else
        {
            return StatusCode(500, new { message = "Failed to update patient medical record." });
        }
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "An error occurred while updating the patient medical record.");
        return StatusCode(500, new { message = "An error occurred while updating the patient medical record." });
    }
}


    [HttpGet]
    public async Task<IActionResult> GetAllPatientMedicalRecords()
    {
        try
        {
            var patientMedicalRecords = await _patientMedicalRecordService.GetAllPatientMedicalRecordsAsync();
            if (patientMedicalRecords == null || !patientMedicalRecords.Any())
            {
                return NotFound(new { message = "No patient medical records found." });
            }
            return Ok(new { message = "All patient medical records fetched successfully.", data = patientMedicalRecords });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching patient medical records.");
            return StatusCode(500, new { message = "An error occurred while fetching patient medical records." });
        }
    }

    [HttpPost]
public async Task<IActionResult> SyncPatientRecordsToNodeJs()
{
    try
    {
        var filterDto = new PatientFilterDto();
        // Fetch all patients
        var patientsResponse = await _patientService.GetFilteredPatientsAsync(filterDto);
        if (patientsResponse == null || !patientsResponse.Any())
        {
            return NotFound(new { message = "No patients found to sync." });
        }

        // Extract record numbers and full names
        var recordsToSync = patientsResponse.Select(patient => new
        {
            RecordNumber = patient.Patient.RecordNumber,
            FullName = patient.UserId
        }).ToList();

        // Send data to the Node.js backend
        var isSuccess = await _patientMedicalRecordService.SyncRecordsToNodeJsAsync(recordsToSync);

        if (isSuccess)
        {
            return Ok(new { message = "Patient records synced successfully to the Node.js backend." });
        }

        return StatusCode(500, new { message = "Failed to sync patient records to the Node.js backend." });
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "An error occurred while syncing patient records to the Node.js backend.");
        return StatusCode(500, new { message = "An error occurred while syncing patient records.", details = ex.Message });
    }
}
[HttpGet("{recordNumber}/download")]
    public async Task<IActionResult> DownloadPatientMedicalRecord(string recordNumber)
    {
        try
        {
            // Chama o serviço para obter os bytes do arquivo JSON
            var fileBytes = await _patientMedicalRecordService.GetPatientMedicalRecordForDownloadAsync(recordNumber);
            // Retorna o arquivo JSON para o download
            return File(fileBytes, "application/json", $"{recordNumber}_patient_medical_record.json");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching the patient medical record.");
            return StatusCode(500, new { message = "An error occurred while fetching the patient medical record." });
        }
    }

}