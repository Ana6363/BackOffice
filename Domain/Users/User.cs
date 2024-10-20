using System.Runtime.CompilerServices;
using BackOffice.Domain.Patients;
using BackOffice.Domain.Shared;

namespace BackOffice.Domain.Users
{
    public class User : Entity<UserId>, IAggregateRoot
    {
        public string Role { get; private set; }
        public bool Active { get; set; }

        public PhoneNumber PhoneNumber { get;set; }

        public Name FirstName { get; private set; }
        public Name LastName { get; private set; }
        public Name FullName {get; private set;}

        public string? ActivationToken { get; set; }
        public DateTime? TokenExpiration { get; set; }

        public bool IsToBeDeleted { get; private set; }

        private User() { }

        public User(string email, string role,PhoneNumber phoneNumber, Name firstName, Name lastName,Name fullName)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new BusinessRuleValidationException("Email cannot be empty.");
            
            if (string.IsNullOrWhiteSpace(role))
                throw new BusinessRuleValidationException("Role cannot be empty.");
                
            this.Id = new UserId(email);
            this.Role = role;
            this.PhoneNumber = phoneNumber ?? throw new BusinessRuleValidationException("PhoneNumber cannot be null");
            this.FirstName = firstName ?? throw new BusinessRuleValidationException("First name cannot be null.");
            this.LastName = lastName ?? throw new BusinessRuleValidationException("Last name cannot be null.");
            this.FullName = fullName ?? throw new BusinessRuleValidationException("Full name cannot be null.");
            this.Active = false;
            this.IsToBeDeleted = false;
        }

        public void GenerateActivationToken()
        {
            this.ActivationToken = Guid.NewGuid().ToString(); // Generate a unique token
            this.TokenExpiration = DateTime.UtcNow.AddHours(24);
        }

        public bool IsActivationTokenValid()
        {
            return DateTime.UtcNow <= this.TokenExpiration; // Check if the token is still valid
        }

        public void ChangeRole(string role)
        {
            if (!this.Active)
                throw new BusinessRuleValidationException("It is not possible to change the role of an inactive user.");

            if (string.IsNullOrWhiteSpace(role))
                throw new BusinessRuleValidationException("Role cannot be empty.");

            this.Role = role;
        }

        public void MarkAsInactive()
        {
            this.Active = false;
        }

        public void MarkAsActive()
        {
            this.Active = true;
        }

        public void MarkDeleteAsActive(){
            this.IsToBeDeleted = true;
        }

        public void UpdateName(Name firstName, Name lastName)
        {
            FirstName = firstName ?? throw new BusinessRuleValidationException("First name cannot be null.");
            LastName = lastName ?? throw new BusinessRuleValidationException("Last name cannot be null.");
        }
    }
}
