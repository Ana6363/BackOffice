namespace BackOffice.Domain.RoomTypes
{
    public class RoomDescription
    {
        public string Value { get; }
        public RoomDescription(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Description cannot be empty.", nameof(description));
            Value = description;
        }
        public override string ToString()
        {
            return Value;
        }
    }
}
