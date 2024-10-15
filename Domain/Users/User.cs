using BackOffice.Domain.Shared;

namespace BackOffice.Domain.Users
{
    public class User : Entity<UserId>, IAggregateRoot
    {

        public string Role { get;  private set; }

        public bool Active{ get;  private set; }

        private User()
        {
            this.Active = true;
        }

        public User(string email, string role)
        {
            this.Id = new UserId(email);
            this.Role = role;
            this.Active = true;
        }

        public void ChangeRole(string role)
        {
            if (!this.Active)
                throw new BusinessRuleValidationException("It is not possible to change the description to an inactive family.");
            this.Role = role;
        }
        public void MarkAsInative()
        {
            this.Active = false;
        }
    }
}