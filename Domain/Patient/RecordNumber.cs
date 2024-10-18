using System;
using BackOffice.Domain.Shared;

namespace BackOffice.Domain.Patients
{
    public class RecordNumber : EntityId
    {
        public RecordNumber(string number) : base(number)
        {
            if (string.IsNullOrWhiteSpace(number))
                throw new ArgumentException("Record number cannot be empty.", nameof(number));
        }

        protected override object CreateFromString(string text)
        {
            return text; 
        }

        public override string AsString()
        {
            return (string)base.Value;
        }

        public override bool Equals(object obj)
        {
            if (obj is RecordNumber other)
            {
                return string.Equals(Value as string, other.Value as string, StringComparison.Ordinal);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Value?.GetHashCode() ?? 0;
        }
    }
}
