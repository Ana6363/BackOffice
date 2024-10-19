using BackOffice.Domain.Shared;
using BackOffice.Domain.Users;

namespace BackOffice.Domain.Patients
{
    public class Patient : Entity<RecordNumber>, IAggregateRoot
    {
        public RecordNumber RecordNumber { get; set; }
        public DateOfBirth DateOfBirth { get; private set; }
        public PhoneNumber PhoneNumber { get; private set; }
        public PhoneNumber EmergencyContact { get; private set; }
        public Gender Gender { get; private set; }
        public UserId UserId { get; private set; }
        public bool IsToBeDeleted { get; private set; }

        private Patient() { }
        public Patient(RecordNumber recordNumber, UserId userId, DateOfBirth dateOfBirth, PhoneNumber phoneNumber, PhoneNumber emergencyContact, Gender gender, bool isToBeDeleted)
        {
            RecordNumber = recordNumber ?? throw new BusinessRuleValidationException("Record number cannot be null."); // Use the property here
            UserId = userId ?? throw new BusinessRuleValidationException("User ID cannot be null.");
            DateOfBirth = dateOfBirth ?? throw new BusinessRuleValidationException("Date of birth cannot be null.");
            PhoneNumber = phoneNumber ?? throw new BusinessRuleValidationException("Patient contact cannot be null.");
            EmergencyContact = emergencyContact ?? throw new BusinessRuleValidationException("Emergency contact cannot be null.");
            Gender = gender ?? throw new BusinessRuleValidationException("Gender cannot be null.");
            IsToBeDeleted = isToBeDeleted; // Direct assignment since bool is a value type and cannot be null
        }
    }
}
