using System;

namespace BackOffice.Domain.Shared
{
    public class Text
    {
        public string Value { get; private set; }
        public const int MaxLength = 500; // Example max length, adjust as needed

        public Text(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new BusinessRuleValidationException("Text cannot be empty or whitespace.");

            if (value.Length > MaxLength)
                throw new BusinessRuleValidationException($"Text cannot exceed {MaxLength} characters.");

            Value = value;
        }

        public override string ToString()
        {
            return Value;
        }

        public override bool Equals(object obj)
        {
            if (obj is not Text other)
                return false;

            return Value == other.Value;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
}
