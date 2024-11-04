namespace Healthcare.Domain.ValueObjects
{
    public class Equipment
    {
        public string Name { get; private set; }

        public Equipment(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Equipment name is required.");

            Name = name;
        }
    }
}
