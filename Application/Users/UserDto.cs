using System;

namespace BackOffice.Domain.Users
{
    public class UserDto
    {
        public string? Id { get; set; }

        public required string Role { get; set; }

        public bool? Active { get; set; }

        public int PhoneNumber { get; set; }

        public required string FirstName { get; set; }
        public required string LastName { get; set; } 
        public required string FullName { get; set;} 
        public string? ActivationToken { get; set; }

        public DateTime? TokenExpiration { get; set; }

        public bool IsToBeDeleted { get; set; }
    }
}
