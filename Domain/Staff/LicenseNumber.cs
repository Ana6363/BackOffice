using System;
using System.Text.RegularExpressions;

namespace BackOffice.Domain.Staff
{
    public class LicenseNumber
    {
        private const string LicenseNumberPattern = @"^[a-zA-Z0-9]{10}$";
        public string Value { get; private set; }

        public LicenseNumber(string number)
        {
            if (!IsValidLicenseNumber(number))
                throw new ArgumentException("Invalid license number format. It must be a 10-character alphanumeric string.", nameof(number));

            Value = number;
        }

        public string AsString()
        {
            return Value;
        }

        public override bool Equals(object obj)
        {
            if (obj is LicenseNumber other)
            {
                return string.Equals(Value, other.Value, StringComparison.Ordinal);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Value?.GetHashCode() ?? 0;
        }

        private static bool IsValidLicenseNumber(string licenseNumber)
        {
            return Regex.IsMatch(licenseNumber, LicenseNumberPattern);
        }
    }
}
