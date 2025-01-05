using System.Collections.Generic;
using Healthcare.Domain.ValueObjects;
using Healthcare.Domain.Enums;

namespace Healthcare.Domain
{
    public class SurgeryRoom
    {
        public RoomNumber RoomNumber { get; private set; }
        public RoomType Type { get; private set; }
        public Capacity Capacity { get; private set; }
        public RoomStatus Status { get; private set; }
     

        public SurgeryRoom(RoomNumber roomNumber, RoomType type, Capacity capacity, RoomStatus status)
        {
            RoomNumber = roomNumber;
            Type = type;
            Capacity = capacity;
            Status = status;
        }

        private void UpdateStatus(RoomStatus newStatus)
        {
            Status = newStatus;
        }
    }
}
