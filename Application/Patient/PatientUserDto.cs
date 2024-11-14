using BackOffice.Infrastructure.Patients;

public class PatientUserInfoDto
{
    public PatientDataModel Patient { get; set; }
    public string UserId { get; set; }
    public  int PhoneNumber { get; set; }
    public bool IsToBeDeleted { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string FullName { get; set; }
}
