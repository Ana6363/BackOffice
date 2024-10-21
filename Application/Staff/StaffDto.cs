using System;
using System.Collections.Generic;

namespace BackOffice.Domain.Staff
{
    public class StaffDto
    {
        public required string LicenseNumber { get; set; }
        public required string Specialization { get; set; }
        public int? PhoneNumber { get; set; }
        public List<SlotDto> AvailableSlots { get; set; } = new List<SlotDto>();
        public bool Status { get; set; }
    }
}
