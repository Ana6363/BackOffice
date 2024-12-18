public class Description
{
    public string Value { get; }

    public Description(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Description cannot be empty.", nameof(value));

        Value = value;
    }

    public override string ToString()
    {
        return Value;
    }
}
