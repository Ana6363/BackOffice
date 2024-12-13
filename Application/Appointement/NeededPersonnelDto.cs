using System;

namespace BackOffice.Domain.Staff
{
    public class NeededPersonnelDto
{
    public string Specialization{ get; set; }
    public string StaffId { get; set; }

    public NeededPersonnelDto() {}

    public NeededPersonnelDto(string specialization, string staffId)
    {
        Specialization = specialization;
        StaffId = staffId;
    }
}
}