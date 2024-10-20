using System;
using System.Collections.Generic;

namespace BackOffice.Domain.Staff
{
    public class StaffDto
    {
        public required string LicenseNumber { get; set; }
        public required string Specialization { get; set; }
        public required string Email { get; set; }
        public required int PhoneNumber { get; set; }
        public List<SlotDto> AvailableSlots { get; set; } = new List<SlotDto>();
    }
}
