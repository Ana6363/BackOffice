using BackOffice.Domain.Shared;

namespace BackOffice.Domain.Specialization
{
    public class Specialization : Entity<Specializations>, IAggregateRoot
    {

        public Specializations Id { get; set; }

        public Description description { get; set; }
        private Specialization() { }

        public Specialization(Specializations name, Description description)
        {
            Id = name ?? throw new BusinessRuleValidationException("Specialization name cannot be null.");
            description = description ?? throw new BusinessRuleValidationException("Specialization description cannot be null.");
        }
    }
}
