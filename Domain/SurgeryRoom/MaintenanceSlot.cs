using System;

namespace Healthcare.Domain.ValueObjects
{
    public class MaintenanceSlot
    {
        public DateTime Start { get; private set; }
        public DateTime End { get; private set; }

        public MaintenanceSlot(DateTime start, DateTime end)
        {
            if (end <= start)
                throw new ArgumentException("End time must be after start time.");

            Start = start;
            End = end;
        }
    }
}
