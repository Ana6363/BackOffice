using BackOffice.Domain.Shared;

namespace BackOffice.Domain.OperationType;
public class OperationTime
{
    public int time { get; }

    public OperationTime(){}

    public OperationTime(int time)
    {
        if(time < 0){
            throw new BusinessRuleValidationException("Duration of operations cannot be negative.");
        }    
        if (time > 500)
        {
            throw new BusinessRuleValidationException("Duration of operations cannot exceed 500 minutes");
        }    
        this.time = time;

    }

    public float AsInt()
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