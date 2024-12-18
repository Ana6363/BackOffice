namespace BackOffice.Domain.RoomTypes
{
    public class SurgerySuitability
    {
        public enum Suitability {
            SUITABLE,
            NOTSUITABLE
        }

        public Suitability Value { get; private set; }
        public SurgerySuitability(Suitability suitability)
        {
            Value = suitability;
        }

        public override string ToString()
        {
            return Value.ToString();
        }

    }
}
