using BackOffice.Domain.Shared;
using System;
using System.Text.RegularExpressions;

namespace BackOffice.Domain.RoomTypes
{
    public class InternalCode : EntityId
    {
        private const int RequiredLength = 8;
        private static readonly Regex ValidCodeRegex = new Regex(@"^[a-zA-Z0-9\-]{8}$"); // Incluímos '-' na regex

        public string Value { get; private set; }

        private InternalCode(string value) : base(value)
        {
            Value = value;
        }

        public static InternalCode Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Internal code cannot be null or empty.");

            if (value.Length != RequiredLength)
                throw new ArgumentException($"Internal code must be exactly {RequiredLength} characters long.");

            if (!ValidCodeRegex.IsMatch(value))
                throw new ArgumentException("Internal code can only contain letters, numbers, and dashes ('-').");

            return new InternalCode(value);
        }

        public override string AsString()
        {
            return Value;
        }

        protected override object CreateFromString(string text)
        {
            return text;
        }

        public override bool Equals(object obj)
        {
            if (obj is InternalCode other)
            {
                return Value.Equals(other.Value, StringComparison.Ordinal);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString() => Value;
    }
}
