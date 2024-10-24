namespace BackOffice.Domain.OperationRequest
{
    public class Status
    {
        public enum StatusType
        {
            PENDING,
            ACCEPTED,
            REJECTED
        }

        public StatusType Value { get; private set; }

        public Status() { }
        public Status(StatusType type) { 
            Value = type;
        }

        public override string ToString()
        {
            return Value.ToString();
        }

    }
}
