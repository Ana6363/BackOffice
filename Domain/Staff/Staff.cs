using System;
using System.Collections.Generic;
using BackOffice.Domain.Shared;

namespace BackOffice.Domain.Staff
{
    public class Staff : Entity<LicenseNumber>, IAggregateRoot
    {
        public Specializations Specialization { get; private set; }
        public Email Email { get; private set; }
        public PhoneNumber PhoneNumber { get; private set; }

        public List<Slots> AvailableSlots { get; private set; } = new List<Slots>();

        public Staff(LicenseNumber licenseNumber, Specializations specialization, Email email, PhoneNumber phoneNumber, List<Slots> slots)
        {
            Id = licenseNumber ?? throw new BusinessRuleValidationException("License number cannot be null");
            Specialization = specialization ?? throw new BusinessRuleValidationException("Specialization cannot be null");
            Email = email ?? throw new BusinessRuleValidationException("Email cannot be null");
            PhoneNumber = phoneNumber ?? throw new BusinessRuleValidationException("PhoneNumber cannot be null");
            
            if (slots == null || slots.Count == 0)
            {
                throw new BusinessRuleValidationException("Staff must have at least one available slot.");
            }

            AvailableSlots = slots;
        }

        public Staff() { }

        public void AddSlot(Slots slot)
        {
            if (slot == null)
            {
                throw new BusinessRuleValidationException("Slot cannot be null.");
            }

            // Check for slot conflicts before adding
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

        public override string ToString()
        {
            return $"Staff: {Email}, Specialization: {Specialization}, Slots: {string.Join(", ", AvailableSlots)}";
        }
    }
}
