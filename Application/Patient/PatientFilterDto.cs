using Xunit;

public class PatientFilterDto
{
    public string? UserId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? FullName { get; set; }

    public int? PhoneNumber { get; set; }
    public bool? IsToBeDeleted { get; set; }
    public string? RecordNumber { get; set; }

    public PatientFilterDto(string? userId = null, int? phoneNumber = null,string? firstName = null,string? lastName=null,string? fullname =null, bool? isToBeDeleted = null,string? recordNumber = null)
    {
        UserId = userId;
        PhoneNumber = phoneNumber;
        PhoneNumber = PhoneNumber;
        IsToBeDeleted = isToBeDeleted;
        RecordNumber = recordNumber;
    }

    public PatientFilterDto() { }
}
