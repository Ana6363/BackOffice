using System;


namespace BackOffice.Domain.Users
{
    public class UserDto
    {
        public String Id { get; set; }

        public string Role { get; set; }

        public bool Active { get; set;}
    }
}