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
        private readonly List<Equipment> _assignedEquipment;
        public IReadOnlyCollection<Equipment> AssignedEquipment => _assignedEquipment.AsReadOnly();
        public RoomStatus CurrentStatus { get; private set; }
        private readonly List<MaintenanceSlot> _maintenanceSlots;
        public IReadOnlyCollection<MaintenanceSlot> MaintenanceSlots => _maintenanceSlots.AsReadOnly();

        public SurgeryPhase PreparationPhase { get; private set; }
        public SurgeryPhase SurgeryPhase { get; private set; }
        public SurgeryPhase CleaningPhase { get; private set; }

        public SurgeryRoom(RoomNumber roomNumber, RoomType type, Capacity capacity, List<Equipment> assignedEquipment)
        {
            RoomNumber = roomNumber;
            Type = type;
            Capacity = capacity;
            _assignedEquipment = assignedEquipment ?? new List<Equipment>();
            CurrentStatus = RoomStatus.Available;
            _maintenanceSlots = new List<MaintenanceSlot>();
        }

        public void SetPreparationPhase(SurgeryPhase phase)
        {
            if (phase.PhaseName != "Preparation")
                throw new ArgumentException("Invalid phase name. Expected 'Preparation'.");
            
            PreparationPhase = phase;
        }

        public void SetSurgeryPhase(SurgeryPhase phase)
        {
            if (phase.PhaseName != "Surgery")
                throw new ArgumentException("Invalid phase name. Expected 'Surgery'.");
            
            SurgeryPhase = phase;
        }

        public void SetCleaningPhase(SurgeryPhase phase)
        {
            if (phase.PhaseName != "Cleaning")
                throw new ArgumentException("Invalid phase name. Expected 'Cleaning'.");
            
            CleaningPhase = phase;
        }

        public void BeginEvent()
        {
            if (CurrentStatus == RoomStatus.Available)
            {
                UpdateStatus(RoomStatus.Occupied);
            }
            else
            {
                throw new InvalidOperationException("Room is currently occupied or under maintenance.");
            }
        }

        public void EndEvent()
        {
            if (CurrentStatus == RoomStatus.Occupied)
            {
                UpdateStatus(RoomStatus.Available);
            }
        }

        private void UpdateStatus(RoomStatus newStatus)
        {
            CurrentStatus = newStatus;
        }
    }
}
