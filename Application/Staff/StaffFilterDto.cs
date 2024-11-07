public class StaffFilterDto
{
    public string? StaffId { get; set; }
    public int? PhoneNumber { get; set; }
    public string? Specialization { get; set; }
    public bool? Status { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? FullName { get; set; }

    public StaffFilterDto(string? staffId, int? phoneNumber,string? specialization,bool? status, string? firstName, string? lastName, string? fullName)
    {
        StaffId = staffId;
        PhoneNumber = phoneNumber;
        Specialization = specialization;
        Status = status;
        FirstName = firstName;
        LastName = lastName;
        FullName = fullName;
    }
}
