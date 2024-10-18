using BackOffice.Domain.Shared;

namespace BackOffice.Domain.Patients
{
    public class DateOfBirth
    {
        public DateTime Value { get; private set; }

        public DateOfBirth(DateTime dateOfBirth)
        {
            if (dateOfBirth > DateTime.UtcNow)
                throw new BusinessRuleValidationException("Date of birth cannot be in the future.");

            Value = dateOfBirth;
        }
    }
}
