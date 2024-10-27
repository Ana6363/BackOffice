using BackOffice.Domain.Shared;

namespace BackOffice.Domain.OperationType;
public class OperationTime
{
    public float time { get; }

    public OperationTime(){}

    public OperationTime(float time)
    {
        if(time < 0){
            throw new BusinessRuleValidationException("Duration of operations cannot be negative.");
        }    
        if (time > 8)
        {
            throw new BusinessRuleValidationException("Duration of operations cannot exceed 8 hours");
        }    
        this.time = time;

    }

    public float AsFloat()
    {
        return time;
    }

    override
    public int GetHashCode()
    {
        return time.GetHashCode();
    }

    override
    public bool Equals(Object? obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        OperationTime operationTime = (OperationTime)obj;
        return time == operationTime.time;
    }
}