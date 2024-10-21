using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BackOffice.Infrastructure.Staff
{
    public class StaffDataModel
    {
        [Key]
        public string StaffId { get; set; }
        public string LicenseNumber { get; set; } 

        public string Specialization { get; set; } 

        public string Email { get; set; } 

        public List<AvailableSlotDataModel> AvailableSlots { get; set; }
        public bool Status { get; set; }
    }
}    