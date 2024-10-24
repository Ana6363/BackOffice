using System;
using BackOffice.Domain.Shared;

namespace BackOffice.Domain.Staff
{
    public class StaffId : EntityId
    {
        private const int SequentialNumberLength = 5;
        public string StaffType { get; private set; }
        public int RecruitmentYear { get; private set; }
        public int SequentialNumber { get; private set; }

        public StaffId(string role, int recruitmentYear, int sequentialNumber)
            : base(GenerateStaffId(GetStaffType(role), recruitmentYear, sequentialNumber))
        {
            StaffType = GetStaffType(role);
            RecruitmentYear = recruitmentYear;
            SequentialNumber = sequentialNumber;

            if (recruitmentYear < 1900 || recruitmentYear > DateTime.Now.Year)
                throw new ArgumentException("Invalid recruitment year.", nameof(recruitmentYear));
            if (sequentialNumber < 0 || sequentialNumber >= Math.Pow(10, SequentialNumberLength))
                throw new ArgumentException($"Invalid sequential number. It must be a number between 00000 and 99999.", nameof(sequentialNumber));
        }

        // Constructor for parsing an existing StaffId
        public StaffId(string id)
            : base(id)
        {
            // Parse the StaffType, RecruitmentYear, and SequentialNumber from the input string
            if (id.Length != 10)
            {
                throw new ArgumentException("Invalid StaffId format. It must be 10 characters long.", nameof(id));
            }

            StaffType = id.Substring(0, 1);
            RecruitmentYear = int.Parse(id.Substring(1, 4));
            SequentialNumber = int.Parse(id.Substring(5, 5));

            ValidateInputs(RecruitmentYear, SequentialNumber);
        }

        private void ValidateInputs(int recruitmentYear, int sequentialNumber)
        {
            if (recruitmentYear < 1900 || recruitmentYear > DateTime.Now.Year)
                throw new ArgumentException("Invalid recruitment year.", nameof(recruitmentYear));
            if (sequentialNumber < 0 || sequentialNumber >= Math.Pow(10, SequentialNumberLength))
                throw new ArgumentException($"Invalid sequential number. It must be a number between 00000 and 99999.", nameof(sequentialNumber));
        }

        private static string GetStaffType(string role)
        {
            return role switch
            {
                "Doctor" => "D",
                "Nurse" => "N",
                _ => "O"
            };
        }

        private static string GenerateStaffId(string staffType, int recruitmentYear, int sequentialNumber)
        {
            return $"{staffType}{recruitmentYear:D4}{sequentialNumber:D5}";
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
            if (obj is StaffId other)
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
