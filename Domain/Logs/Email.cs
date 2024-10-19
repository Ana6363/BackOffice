using BackOffice.Domain.Shared;

public class Email
{
    public string Value { get; private set; }

    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || !value.Contains("@"))
            throw new BusinessRuleValidationException("Invalid email address.");
        Value = value;
    }

    public override bool Equals(object obj)
    {
        if (obj is not Email other)
            return false;

        return Value.Equals(other.Value, StringComparison.OrdinalIgnoreCase);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode(StringComparison.OrdinalIgnoreCase);
    }

    public override string ToString()
    {
        return Value;
    }
}
