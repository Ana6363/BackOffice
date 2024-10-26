using System;
using BackOffice.Domain.Users;

namespace BackOffice.Domain.Staff
{
    public class Slots
    {
        public DateTime StartTime { get; private set; }
        public DateTime EndTime { get; private set; }

        public Slots(DateTime startTime, DateTime endTime)
        {
            if (endTime <= startTime)
            {
                throw new ArgumentException("End time must be after start time.");
            }

            StartTime = startTime;
            EndTime = endTime;
        }

        public bool ConflictsWith(Slots otherSlot)
        {
            // Check if the slots overlap
            return StartTime < otherSlot.EndTime && EndTime > otherSlot.StartTime;
        }

        public bool IsWithinSlot(DateTime dateTime)
        {
            return StartTime <= dateTime && EndTime >= dateTime;
        }

        public override bool Equals(object obj)
        {
            if (obj is Slots otherSlot)
            {
                return StartTime == otherSlot.StartTime && EndTime == otherSlot.EndTime;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(StartTime, EndTime);
        }

        public override string ToString()
        {
            return $"{StartTime:yyyy-MM-dd HH:mm} - {EndTime:yyyy-MM-dd HH:mm}";
        }
    }
}
