using System;
using System.Collections.Generic;

namespace Healthcare.Domain.ValueObjects
{
    public class SurgeryPhase
    {
        private static readonly HashSet<string> AllowedPhases = new HashSet<string>
        {
            "Preparation", "Surgery", "Cleaning"
        };

        public string PhaseName { get; private set; }
        public TimeSpan Duration { get; private set; }
        public DateTime StartTime { get; private set; }
        public DateTime EndTime => StartTime + Duration;

        public SurgeryPhase(string phaseName, TimeSpan duration, DateTime startTime)
        {
            if (!AllowedPhases.Contains(phaseName))
                throw new ArgumentException($"Invalid phase name: {phaseName}. Allowed values are: {string.Join(", ", AllowedPhases)}.");
            
            if (duration.TotalMinutes <= 0)
                throw new ArgumentException("Duration must be greater than zero.");
            
            PhaseName = phaseName;
            Duration = duration;
            StartTime = startTime;
        }
    }
}
