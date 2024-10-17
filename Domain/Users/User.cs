using BackOffice.Domain.Shared;

namespace BackOffice.Domain.Users
{
    public class User : Entity<UserId>, IAggregateRoot
    {
        public string Role { get; private set; }
        public bool Active { get;  set; }

        // Private constructor for EF Core
        private User() 
        {
            this.Active = true;
        }

        public User(string email, string role)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new BusinessRuleValidationException("Email cannot be empty.");
            
            if (string.IsNullOrWhiteSpace(role))
                throw new BusinessRuleValidationException("Role cannot be empty.");
                
            this.Id = new UserId(email); // Set UserId using email
            this.Role = role;
            this.Active = true; // Default to active
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
    }
}
