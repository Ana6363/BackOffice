namespace BackOffice.Domain.OperationRequest
{

    public class Priority
    {
        public enum PriorityType
        {
            LOW,
            MEDIUM,
            HIGH
        }

        public PriorityType Value { get; private set; }

        public Priority(PriorityType priority)
        {
            Value = priority;
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
