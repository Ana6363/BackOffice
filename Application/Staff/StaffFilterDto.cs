public class StaffFilterDto
{
    public string? StaffId { get; set; }
    public int? PhoneNumber { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? FullName { get; set; }

    public StaffFilterDto(string? staffId, int? phoneNumber, string? firstName, string? lastName, string? fullName)
    {
        StaffId = staffId;
        PhoneNumber = phoneNumber;
        FirstName = firstName;
        LastName = lastName;
        FullName = fullName;
    }
}
