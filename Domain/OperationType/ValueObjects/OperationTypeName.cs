using BackOffice.Domain.Shared;

namespace BackOffice.Domain.OperationType;
public class OperationTypeName : IValueObject
{
    public string Name { get; }

    public OperationTypeName(string name){

        if (string.IsNullOrEmpty(name))
        {
            throw new BusinessRuleValidationException("Operation Type name can´t be null or empty");
        }
        
        if (double.TryParse(name, out _))
        {
            throw new BusinessRuleValidationException("Operation Type name must be a String.");
        }
        Name = name;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Name;
    }
}

