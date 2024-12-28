using System.Text.Json.Serialization;

    public class PatientMedicalRecordDto
    {
        [JsonPropertyName("recordNumber")]
        public int RecordNumber { get; set; }

        [JsonPropertyName("allergies")]
        public string Allergies { get; set; }

        [JsonPropertyName("medicalConditions")]
        public string MedicalConditions { get; set; }

        [JsonPropertyName("fullName")]
        public string FullName { get; set; }
    }