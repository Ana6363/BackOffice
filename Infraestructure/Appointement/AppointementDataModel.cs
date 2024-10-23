using BackOffice.Domain.OperationRequest;
using BackOffice.Domain.Patients;
using BackOffice.Domain.Staff;
using System.ComponentModel.DataAnnotations;

namespace BackOffice.Infraestructure.Appointement
{
    public class AppointementDataModel
    {
        [Key]
        public required Guid AppointementId { get; set; } // Primary key
        public required DateTime Schedule { get; set; }
        public required string Request { get; set; }
        public required string Patient { get; set; }
        public required string Staff { get; set; }


    }
}
