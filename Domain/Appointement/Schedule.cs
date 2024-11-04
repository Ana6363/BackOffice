using BackOffice.Domain.Shared;

namespace BackOffice.Domain.Appointement
{
    public class Schedule
    {
        public DateTime Value { get; private set; }

        public Schedule(DateTime schedule)
        {
            if (schedule < DateTime.UtcNow)
                throw new BusinessRuleValidationException("Schedule cannot be in the past.");

            Value = schedule;
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
