using BackOffice.Domain.Shared;

namespace BackOffice.Domain.OperationRequest
{
    public class DeadLine
    {
        public DateTime Value { get; private set; }

        public DeadLine(DateTime deadLine)
        {
            if(deadLine < DateTime.UtcNow)
                throw new BusinessRuleValidationException("DeadLine cannot be in the past.");
            Value = deadLine;  
        }


        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
