using BackOffice.Domain.Shared;
using BackOffice.Domain.Users;

namespace BackOffice.Domain.Logs
{
    public class Log : Entity<LogId>, IAggregateRoot
    {
        public DateTime Timestamp { get; private set; }
        public ActionType ActionType { get; private set; }
        public Email AffectedUserEmail { get; private set; }
        public Text Details { get; private set; }

        public Log(LogId logId, ActionType actionType, Email affectedUserEmail, Text details)
        {
            Id = logId ?? throw new BusinessRuleValidationException("Log ID cannot be null.");
            Timestamp = DateTime.UtcNow;
            ActionType = actionType ?? throw new BusinessRuleValidationException("Action type cannot be null.");
            AffectedUserEmail = affectedUserEmail ?? throw new BusinessRuleValidationException("Affected user email cannot be null.");
            Details = details ?? throw new BusinessRuleValidationException("Details cannot be null.");
        }

        private Log() { }
    }
}