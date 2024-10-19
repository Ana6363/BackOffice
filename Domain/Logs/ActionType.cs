namespace BackOffice.Domain.Logs
{
    public enum ActionTypeEnum
    {
        Create,
        Update,
        Delete
    }

    public class ActionType
    {
        public ActionTypeEnum Value { get; private set; }

        public ActionType(ActionTypeEnum value)
        {
            Value = value;
        }

        public override bool Equals(object obj)
        {
            if (obj is not ActionType other)
                return false;

            return Value == other.Value;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
