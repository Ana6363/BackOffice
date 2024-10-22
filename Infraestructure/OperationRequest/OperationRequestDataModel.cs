using BackOffice.Domain.Patients;
using System.ComponentModel.DataAnnotations;

namespace BackOffice.Infraestructure.OperationRequest
{
    public class OperationRequestDataModel
    {
        [Key]
        public required Guid RequestId { get; set; } // Primary key
        public required DateTime DeadLine { get; set; }
        public required string Priority { get; set; }
        public required string RecordNumber { get; set; }
        public required string StaffId { get; set; }
        //public required string OperationType { get; set; }

    }
}
