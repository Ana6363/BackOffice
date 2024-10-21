using System.ComponentModel.DataAnnotations;
using BackOffice.Infrastructure.Staff;

public class AvailableSlotDataModel
    {
        [Key]
        public int Id { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        // Foreign key to reference the staff member
        public string StaffId { get; set; }

        // Navigation property to the StaffDataModel
        public StaffDataModel Staff { get; set; }
    }