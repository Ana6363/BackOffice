using System.ComponentModel.DataAnnotations;

namespace BackOffice.Infrastructure.Persistence.Models
{
    public class LogDataModel
    {
        [Key] 
        public string LogId { get; set; } // Storing Guid as a string
        public DateTime Timestamp { get; set; }
        public string ActionType { get; set; } // Enum as a string
        public string AffectedUserEmail { get; set; }
        public string Details { get; set; }
    }
}
