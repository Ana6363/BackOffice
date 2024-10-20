using System;
using System.Text.RegularExpressions;
using BackOffice.Domain.Shared;

namespace BackOffice.Domain.Staff
{
    public class LicenseNumber : EntityId
    {
        private const string LicenseNumberPattern = @"^[A-Z0-9]{6,10}$";
        public LicenseNumber(string number) : base(number)
        {
            if (!IsValidLicenseNumber(number))
                throw new ArgumentException("Invalid license number format.", nameof(number));
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
            if (obj is LicenseNumber other)
            {
                return string.Equals(Value as string, other.Value as string, StringComparison.Ordinal);
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
