using System;
using System.ComponentModel.DataAnnotations;

namespace BackOffice.Infrastructure.Patients
{
    public class PatientDataModel
    {
        [Key]
        public required string RecordNumber { get; set; } // Primary key

        public required int PhoneNumber { get; set; }

        public required string UserId { get; set; }

        public required DateTime DateOfBirth { get; set; }

        public required int EmergencyContact { get; set; }

        public required string Gender { get; set; }

        public required bool IsToBeDeleted { get; set; }
    }
}
