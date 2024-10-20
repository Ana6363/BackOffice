using System;
using System.Collections.Generic;
using System.Linq;
using BackOffice.Domain.Staff;
using BackOffice.Infrastructure.Staff;
using Microsoft.Extensions.Configuration;  // Add this namespace

namespace BackOffice.Application.Staffs
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

                AvailableSlots = domainModel.AvailableSlots.Select(slot => ToDto(slot)).ToList(),
                Status = domainModel.Status.IsActive
            };
        }

        // DTO to Domain Model (with IConfiguration)
        public static Staff ToDomain(StaffDto dto, IConfiguration configuration)
        {
            var slots = dto.AvailableSlots.Select(ToDomain).ToList();
            var specialization = Specializations.FromString(dto.Specialization);
            var status = dto.Status ? StaffStatus.Active() : StaffStatus.Inactive();

            return new Staff(
                new LicenseNumber(dto.LicenseNumber),
                specialization,
                new StaffEmail(dto.LicenseNumber, dto.Email, configuration),  // Pass configuration here
                slots,
                status
            );
        }

        // DataModel to Domain Model (with IConfiguration)
        public static Staff ToDomain(StaffDataModel dataModel, IConfiguration configuration)
        {
            var slots = dataModel.AvailableSlots.Select(ToDomain).ToList();
            var specialization = Specializations.FromString(dataModel.Specialization);
            var status = dataModel.Status ? StaffStatus.Active() : StaffStatus.Inactive();

            return new Staff(
                new LicenseNumber(dataModel.LicenseNumber),
                specialization,
                new StaffEmail(dataModel.LicenseNumber, dataModel.Email, configuration),  // Pass configuration here
                slots,
                status
            );
        }

        // Domain Model to DataModel
        public static StaffDataModel ToDataModel(Staff domainModel)
        {
            return new StaffDataModel
            {
                LicenseNumber = domainModel.Id.AsString(),
                Specialization = domainModel.Specialization.ToString(),
                Email = domainModel.Email.Value,  // Map email
                AvailableSlots = domainModel.AvailableSlots.Select(ToDataModel).ToList(),
                Status = domainModel.Status.IsActive
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
                AvailableSlots = dataModel.AvailableSlots.Select(slot => ToDto(slot)).ToList(),
                Status = dataModel.Status
            };
        }

        // Slot Mapping: AvailableSlotDataModel to SlotDto
        public static SlotDto ToDto(AvailableSlotDataModel slotDataModel)
        {
            return new SlotDto
            {
                StartTime = slotDataModel.StartTime,
                EndTime = slotDataModel.EndTime
            };
        }

        // Slot Mapping: Slots (Domain) to SlotDto
        public static SlotDto ToDto(Slots domainSlot)
        {
            return new SlotDto
            {
                StartTime = domainSlot.StartTime,
                EndTime = domainSlot.EndTime
            };
        }

        // Slot Mapping: SlotDto to Slots (Domain)
        public static Slots ToDomain(SlotDto dto)
        {
            return new Slots(dto.StartTime, dto.EndTime);
        }

        // Slot Mapping: AvailableSlotDataModel to Slots (Domain)
        public static Slots ToDomain(AvailableSlotDataModel dataModel)
        {
            return new Slots(dataModel.StartTime, dataModel.EndTime);
        }

        // Slot Mapping: Slots (Domain) to AvailableSlotDataModel
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
