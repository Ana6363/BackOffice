public class PatientDto
{
    public string? RecordNumber { get; set; } // Unique identifier for the patient
    public DateTime DateOfBirth { get; set; }
    public int PhoneNumber { get; set; } // Patient's phone number
    public int EmergencyContact { get; set; } // Emergency contact number
    public string Gender { get; set; } // Use valid values for Gender
    public string UserId { get; set; } // Corresponds to UserId

    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string FullName { get; set; }

    public PatientDto(string recordNumber, DateTime dateOfBirth, int phoneNumber, int emergencyContact, string gender, string userId)
    {
        RecordNumber = recordNumber;
        DateOfBirth = dateOfBirth;
        PhoneNumber = phoneNumber;
        EmergencyContact = emergencyContact;
        Gender = gender;
        UserId = userId;
    }
}
