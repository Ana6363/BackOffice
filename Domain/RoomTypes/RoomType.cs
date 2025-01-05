using BackOffice.Domain.Shared;

namespace BackOffice.Domain.RoomTypes
{
    public class RoomType : Entity<InternalCode>, IAggregateRoot
    {
        public InternalCode Id { get; private set; }
        public RoomDesignation Designation { get; private set; }
        public RoomDescription Description { get; private set; }
        public SurgerySuitability SurgerySuitability { get; private set; }

        private RoomType() { }

        public RoomType(string code, RoomDesignation designation, RoomDescription description, SurgerySuitability surgerySuitability)
        {
            Id = InternalCode.Create(code) ?? throw new BusinessRuleValidationException("Invalid room type code.");
            Designation = designation ?? throw new BusinessRuleValidationException("Room type designation cannot be null.");
            Description = description;
            SurgerySuitability = surgerySuitability ?? throw new BusinessRuleValidationException("Surgery suitability is necessary.");
        }
    }
}
