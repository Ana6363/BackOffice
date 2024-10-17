using System;


namespace BackOffice.Domain.Users
{
    public class UserDto
    {
        public required String Id { get; set; }

        public required string Role { get; set; }

        public bool Active { get; set;}

        public string? ActivationToken { get; set;}

        public DateTime? TokenExpiration { get;  set; } 
    }
}