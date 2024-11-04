namespace Healthcare.Domain.ValueObjects
{
    public class RoomType
    {
        public string Value { get; private set; }

        private static readonly HashSet<string> AllowedTypes = new HashSet<string>
        {
            "Operating Room", "Consultation Room", "ICU"
        };

        public RoomType(string value)
        {
            if (!AllowedTypes.Contains(value))
                throw new ArgumentException($"Invalid room type: {value}");

            Value = value;
        }
    }
}
