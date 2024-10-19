namespace BackOffice.Application.Logs
{
    public class LogDto
    {
        public string LogId { get; set; } // Representing the LogId as a string for transfer
        public DateTime Timestamp { get; set; }
        public string ActionType { get; set; } // Enum as string
        public string AffectedUserEmail { get; set; }
        public string Details { get; set; }
    }
}
