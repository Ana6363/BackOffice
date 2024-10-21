using System;
using System.Collections.Generic;
using BackOffice.Domain.Shared;

namespace BackOffice.Domain.Staff
{
    public class Staff : Entity<StaffId>, IAggregateRoot
    {
        public LicenseNumber LicenseNumber { get; private set; }
        public Specializations Specialization { get; private set; }
        public StaffEmail Email { get; private set; }
        public List<Slots> AvailableSlots { get; private set; } = new List<Slots>();
        public StaffStatus Status { get; private set; }

        public Staff(StaffId id,LicenseNumber licenseNumber, Specializations specialization, StaffEmail  email, List<Slots> slots, StaffStatus status)
        {
            Id = id ?? throw new BusinessRuleValidationException("StaffId cannot be null");
            LicenseNumber = licenseNumber ?? throw new BusinessRuleValidationException("License number cannot be null");
            Specialization = specialization ?? throw new BusinessRuleValidationException("Specialization cannot be null");
            Email = email ?? throw new BusinessRuleValidationException("Email cannot be null");
            
            if (slots == null || slots.Count == 0)
            {
                throw new BusinessRuleValidationException("Staff must have at least one available slot.");
            }

            AvailableSlots = slots;
            Status = status ?? throw new BusinessRuleValidationException("Status cannot be null");
        }

        public Staff() { }

        public void AddSlot(Slots slot)
        {
            if (slot == null)
            {
                throw new BusinessRuleValidationException("Slot cannot be null.");
            }

            foreach (var existingSlot in AvailableSlots)
            {
                if (existingSlot.ConflictsWith(slot))
                {
                    throw new BusinessRuleValidationException("New slot conflicts with existing slots.");
                }
            }

            AvailableSlots.Add(slot);
        }

        public void RemoveSlot(Slots slot)
        {
            AvailableSlots.Remove(slot);
        }
        
        public void Deactivate()
        {
            if (Status.IsActive)
            {
                Status = StaffStatus.Inactive();
            }
        }

        public override string ToString()
        {
            return $"Staff: {Email}, Specialization: {Specialization}, Slots: {string.Join(", ", AvailableSlots)}, Status: {Status}";
        }
    }
}
