using BackOffice.Domain.Shared;

namespace BackOffice.Domain.OperationType;
public class OperationTypeName
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

    protected IEnumerable<object> GetEqualityComponents()
    {
        yield return Name;
    }
}

