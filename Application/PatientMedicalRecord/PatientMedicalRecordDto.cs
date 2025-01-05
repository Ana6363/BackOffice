using System.Text.Json.Serialization;

    public class PatientMedicalRecordDto
    {
        [JsonPropertyName("recordNumber")]
        public string RecordNumber { get; set; }

        [JsonPropertyName("allergies")]
        public List<string> Allergies { get; set; }

        [JsonPropertyName("medicalConditions")]
        public List<string> MedicalConditions { get; set; }

        [JsonPropertyName("fullName")]
        public string FullName { get; set; }
    }