namespace Healthcare.Domain.ValueObjects
{
    public class RoomNumber
    {
        public string Value { get; private set; }

        public RoomNumber(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Room number is required.");
            
            Value = value;
        }
    }
}
