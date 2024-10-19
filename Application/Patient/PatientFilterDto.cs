public class PatientFilterDto
{
    public string? UserId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? FullName { get; set; }

    public PatientFilterDto(string? userId = null, string? firstName = null, string? lastName = null, string? fullName = null)
    {
        UserId = userId;
        FirstName = firstName;
        LastName = lastName;
        FullName = fullName;
    }

    public PatientFilterDto() { }
}
