using System;
using System.Collections.Generic;

namespace BackOffice.Domain.Utilities
{
    public class AvailableSlotComparer : IEqualityComparer<AvailableSlotDataModel>
    {
        public bool Equals(AvailableSlotDataModel x, AvailableSlotDataModel y)
        {
            if (x == null || y == null) return false;
            return x.StartTime == y.StartTime && x.EndTime == y.EndTime;
        }

        public int GetHashCode(AvailableSlotDataModel obj)
        {
            return HashCode.Combine(obj.StartTime, obj.EndTime);
        }

        public bool AreSlotsDifferent(List<AvailableSlotDataModel> currentSlots, List<AvailableSlotDataModel> updatedSlots)
        {
            if (currentSlots.Count != updatedSlots.Count)
                return true;

            var comparer = new AvailableSlotComparer();
            
            for (int i = 0; i < currentSlots.Count; i++)
            {
                if (!comparer.Equals(currentSlots[i], updatedSlots[i]))
                {
                    return true;
                }
            }

            return false;
        }

    }
}
