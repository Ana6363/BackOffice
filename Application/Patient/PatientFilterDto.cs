public class PatientFilterDto
{
    public string? UserId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? FullName { get; set; }

    public int? PhoneNumber { get; set; }
    public bool? IsToBeDeleted { get; set; }

    public PatientFilterDto(string? userId = null, int? phoneNumber = null, string? firstName = null, string? lastName = null, string? fullName = null, bool? isToBeDeleted = null)
    {
        UserId = userId;
        PhoneNumber = phoneNumber;
        FirstName = firstName;
        LastName = lastName;
        FullName = fullName;
        PhoneNumber = PhoneNumber;
        IsToBeDeleted = isToBeDeleted;

    }

    public PatientFilterDto() { }
}
