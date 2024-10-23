using BackOffice.Domain.Shared;

namespace BackOffice.Domain.OperationRequest
{
    public class RequestId : EntityId
    {
        public RequestId(Guid id) : base(id)
        {
        }

        public override string AsString()
        {
            return ObjValue.ToString();
        }

        protected override object CreateFromString(string text)
        {
            return new RequestId(Guid.Parse(text));
        }

        public override bool Equals(object obj)
        {
            if (obj is RequestId other)
            {
                return ObjValue.Equals(other.ObjValue);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return ObjValue.GetHashCode();
        }
    }
}
