using BackOffice.Domain.Shared;

namespace BackOffice.Domain.OperationType;

public class OperationTypeId : EntityId 
{
    private readonly string idValue;

    // Constructor that takes a string
    public OperationTypeId(string value) : base(value)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new ArgumentException("ID cannot be null or empty.", nameof(value));
        }
        this.idValue = value;
    }

    // Static method to generate a new ID as a GUID string
    public static OperationTypeId NewId()
    {
        return new OperationTypeId(Guid.NewGuid().ToString());
    }

    // Return ID as string
    public override string AsString()
    {
        return idValue;
    }

    public override bool Equals(object obj)
    {
        return obj is OperationTypeId other && idValue == other.idValue;
    }

    public override int GetHashCode()
    {
        return idValue.GetHashCode();
    }

    // Avoid creating an instance here to prevent the recursive loop
    protected override object CreateFromString(string text)
    {
        return text;
    }

    public override string ToString()
    {
        return idValue;
    }

    // Factory method to create an OperationTypeId from an existing string
    public static OperationTypeId FromString(string id)
    {
        return new OperationTypeId(id);
    }
}
