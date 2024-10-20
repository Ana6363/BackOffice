public class StaffFilterDto
{
    public int? PhoneNumber { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? FullName { get; set; }

    public StaffFilterDto(int? phoneNumber, string? firstName, string? lastName, string? fullName)
    {
        PhoneNumber = phoneNumber;
        FirstName = firstName;
        LastName = lastName;
        FullName = fullName;
    }
}
