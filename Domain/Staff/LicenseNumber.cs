using System;
using System.Text.RegularExpressions;
using BackOffice.Domain.Shared;

namespace BackOffice.Domain.Staff
{
    public class LicenseNumber : EntityId, IEquatable<LicenseNumber>
    {
        public char Role { get; private set; }
        public int Year { get; private set; }
        public int SequentialNumber { get; private set; }

        private static readonly Regex LicenseNumberPattern = new Regex(@"^(N|D|O)(\d{4})(\d{5})$", RegexOptions.Compiled);

        public LicenseNumber(string licenseNumber) : base(licenseNumber)
        {
            if (string.IsNullOrWhiteSpace(licenseNumber))
                throw new ArgumentException("License number cannot be null or empty.", nameof(licenseNumber));

            var match = LicenseNumberPattern.Match(licenseNumber);
            if (!match.Success)
                throw new ArgumentException("Invalid license number format. Expected format: (N | D | O)yyyynnnnn", nameof(licenseNumber));

            Role = match.Groups[1].Value[0]; // 'N', 'D', or 'O'
            Year = int.Parse(match.Groups[2].Value); // 'yyyy'
            SequentialNumber = int.Parse(match.Groups[3].Value); // 'nnnnn'
        }

        protected override object CreateFromString(string text)
        {
            return new LicenseNumber(text); 
        }

        public override string AsString()
        {
            return (string)base.Value;
        }

        public override bool Equals(object obj)
        {
            if (obj is LicenseNumber other)
            {
                return string.Equals(AsString(), other.AsString(), StringComparison.Ordinal);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return AsString().GetHashCode();
        }

        public bool Equals(LicenseNumber? other)
        {
            return other != null && AsString() == other.AsString();
        }
    }
}
