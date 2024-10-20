using System;
using System.Collections.Generic;
using System.Linq;
using BackOffice.Domain.Staff;
using BackOffice.Infrastructure.Staff;

namespace BackOffice.Application.StaffMapper
{
    public static class StaffMapper
    {
        // Domain Model to DTO
        public static StaffDto ToDto(Staff domainModel)
        {
            return new StaffDto
            {
                LicenseNumber = domainModel.Id.AsString(),
                Specialization = domainModel.Specialization.ToString(),
                Email = domainModel.Email.Value,
                PhoneNumber = domainModel.PhoneNumber.Number,
                AvailableSlots = domainModel.AvailableSlots.Select(slot => ToDto(slot)).ToList() // Convert Slots to DTO
            };
        }

        // DTO to Domain Model
        public static Staff ToDomain(StaffDto dto)
        {
            var slots = dto.AvailableSlots.Select(ToDomain).ToList();

            var specialization = Specializations.FromString(dto.Specialization);

            return new Staff(
                new LicenseNumber(dto.LicenseNumber),
                specialization,
                new Email(dto.Email),
                new PhoneNumber(dto.PhoneNumber),
                slots
            );
        }

        // DataModel to Domain Model
        public static Staff ToDomain(StaffDataModel dataModel)
        {
            var slots = dataModel.AvailableSlots.Select(ToDomain).ToList();
            var specialization = Specializations.FromString(dataModel.Specialization);

            return new Staff(
                new LicenseNumber(dataModel.LicenseNumber),
                specialization,
                new Email(dataModel.Email),
                new PhoneNumber(dataModel.PhoneNumber),
                slots
            );
        }

        // Domain Model to DataModel
        public static StaffDataModel ToDataModel(Staff domainModel)
        {
            return new StaffDataModel
            {
                LicenseNumber = domainModel.Id.AsString(),
                Specialization = domainModel.Specialization.ToString(),
                Email = domainModel.Email.Value,
                PhoneNumber = domainModel.PhoneNumber.Number,
                AvailableSlots = domainModel.AvailableSlots.Select(ToDataModel).ToList()
            };
        }

        // DataModel to DTO
        public static StaffDto ToDto(StaffDataModel dataModel)
        {
            return new StaffDto
            {
                LicenseNumber = dataModel.LicenseNumber,
                Specialization = dataModel.Specialization,
                Email = dataModel.Email,
                PhoneNumber = dataModel.PhoneNumber,
                AvailableSlots = dataModel.AvailableSlots.Select(slot => ToDto(slot)).ToList()
            };
        }

        public static SlotDto ToDto(AvailableSlotDataModel slotDataModel)
        {
            return new SlotDto
            {
                StartTime = slotDataModel.StartTime,
                EndTime = slotDataModel.EndTime
            };
        }

        public static SlotDto ToDto(Slots domainSlot)
        {
            return new SlotDto
            {
                StartTime = domainSlot.StartTime,
                EndTime = domainSlot.EndTime
            };
        }

        public static Slots ToDomain(SlotDto dto)
        {
            return new Slots(dto.StartTime, dto.EndTime);
        }

        public static Slots ToDomain(AvailableSlotDataModel dataModel)
        {
            return new Slots(dataModel.StartTime, dataModel.EndTime);
        }


        public static AvailableSlotDataModel ToDataModel(Slots domainSlot)
        {
            return new AvailableSlotDataModel
            {
                StartTime = domainSlot.StartTime,
                EndTime = domainSlot.EndTime
            };
        }
    }
}
