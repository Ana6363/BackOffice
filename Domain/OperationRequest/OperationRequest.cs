using BackOffice.Domain.Patients;
using BackOffice.Domain.Shared;
using BackOffice.Domain.Staff;

namespace BackOffice.Domain.OperationRequest
{
    public class OperationRequest : Entity<RequestId>, IAggregateRoot
    {
        public RequestId RequestId { get; set; }
        public DeadLine DeadLine { get; private set; }
        public Priority Priority { get; set; }
        public RecordNumber Patient { get; set; }
        public LicenseNumber StaffId { get; set; } 

        // Add a type id when created

        private OperationRequest()
        {
        }

        public OperationRequest(RequestId requestId, DeadLine deadLine, Priority priority, RecordNumber patient, LicenseNumber staffId)
        {
            RequestId = requestId ?? throw new BusinessRuleValidationException("Request ID cannot be null.");
            DeadLine = deadLine ?? throw new BusinessRuleValidationException("DeadLine cannot be null.");
            Priority = priority;
            this.Patient = patient ?? throw new BusinessRuleValidationException("Patient cannot be null.");
            StaffId = staffId ?? throw new BusinessRuleValidationException("Staff ID cannot be null.");
        }
    }
}
