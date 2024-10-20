using System;

namespace BackOffice.Domain.Staff
{
    public class StaffStatus : IEquatable<StaffStatus>
    {
        public bool IsActive { get; private set; }

        public StaffStatus(bool isActive)
        {
            IsActive = isActive;
        }

        public static StaffStatus Active()
        {
            return new StaffStatus(true);
        }

        public static StaffStatus Inactive()
        {
            return new StaffStatus(false);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as StaffStatus);
        }

        public bool Equals(StaffStatus other)
        {
            return other != null && IsActive == other.IsActive;
        }

        public override int GetHashCode()
        {
            return IsActive.GetHashCode();
        }

        public override string ToString()
        {
            return IsActive ? "Active" : "Deactivated";
        }
    }
}
