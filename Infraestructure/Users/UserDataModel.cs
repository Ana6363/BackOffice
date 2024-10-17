namespace BackOffice.Infrastructure.Persistence.Models
{
    public class UserDataModel
    {
        public required string Id { get; set; } 

        public required string Role { get; set; } 

        public bool Active { get; set; } 

        public string? ActivationToken { get; set; } 

        public DateTime? TokenExpiration { get; set; }
    }
}
