using BackOffice.Infrastructure.Patients;

public class PatientUserInfoDto
{
    public PatientDataModel Patient { get; set; }
    public string UserId { get; set; }
    public  int PhoneNumber { get; set; }
    public bool IsToBeDeleted { get; set; }
}
