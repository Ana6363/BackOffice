using BackOffice.Domain.Shared;
using BackOffice.Domain.Users;

namespace BackOffice.Domain.Patients
{
    public class Patient : Entity<RecordNumber>, IAggregateRoot
    {
        public RecordNumber RecordNumber { get; set; }
        public DateOfBirth DateOfBirth { get; private set; }
        public PhoneNumber PhoneNumber { get;set; }
        public PhoneNumber EmergencyContact { get;set; }
        public Gender Gender { get; private set; }
        public UserId UserId { get; private set; }

        private Patient() { }
        public Patient(RecordNumber recordNumber, UserId userId, DateOfBirth dateOfBirth, PhoneNumber phoneNumber, PhoneNumber emergencyContact, Gender gender)
        {
            RecordNumber = recordNumber ?? throw new BusinessRuleValidationException("Record number cannot be null.");
            UserId = userId ?? throw new BusinessRuleValidationException("User ID cannot be null.");
            DateOfBirth = dateOfBirth ?? throw new BusinessRuleValidationException("Date of birth cannot be null.");
            PhoneNumber = phoneNumber ?? throw new BusinessRuleValidationException("Patient contact cannot be null.");
            EmergencyContact = emergencyContact ?? throw new BusinessRuleValidationException("Emergency contact cannot be null.");
            Gender = gender ?? throw new BusinessRuleValidationException("Gender cannot be null.");
        }
    }
}
