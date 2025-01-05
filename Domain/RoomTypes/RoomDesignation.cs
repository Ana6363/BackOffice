namespace BackOffice.Domain.RoomTypes
{
    public class RoomDesignation
    {
        public string Value { get; }

        public RoomDesignation(string designation)
        {
            if (string.IsNullOrWhiteSpace(designation))
                throw new ArgumentException("Designation cannot be empty.", nameof(designation));

            if (designation.Length > 100)
                throw new ArgumentException("Designation cannot exceed 100 characters.", nameof(designation));

            Value = designation;
        }

        public override string ToString()
        {
            return Value;
        }
    }
}
