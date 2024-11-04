using BackOffice.Domain.OperationRequest;
using BackOffice.Domain.Shared;
using BackOffice.Domain.Patients;
using BackOffice.Domain.Staff;

namespace BackOffice.Domain.Appointement
{
    public class Appointement : Entity<AppointementId>, IAggregateRoot
    {
        public Schedule Schedule { get; set; }
        public RequestId Request { get; set; }
        public RecordNumber Patient { get; set; }
        public StaffId Staff { get; set; }

        private Appointement()
        {
        }

        public Appointement(AppointementId appointementId, Schedule schedule, RequestId request, RecordNumber patient, StaffId staff)
        {
            Id = appointementId ?? throw new BusinessRuleValidationException("Appointement ID cannot be null."); // Set the Id property directly
            Schedule = schedule ?? throw new BusinessRuleValidationException("Schedule cannot be null.");
            Request = request ?? throw new BusinessRuleValidationException("Request cannot be null.");
            Patient = patient ?? throw new BusinessRuleValidationException("Patient cannot be null.");
            Staff = staff ?? throw new BusinessRuleValidationException("Staff cannot be null.");
        }
    }
}
