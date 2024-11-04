namespace Healthcare.Domain.ValueObjects
{
    public class Capacity
    {
        public int Value { get; private set; }

        public Capacity(int value)
        {
            if (value <= 0)
                throw new ArgumentException("Capacity must be greater than zero.");

            Value = value;
        }
    }
}
