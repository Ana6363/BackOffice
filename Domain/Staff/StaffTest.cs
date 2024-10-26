using System;
using System.Collections.Generic;
using BackOffice.Domain.Shared;
using BackOffice.Domain.Staff;
using Moq;
using Xunit;

namespace BackOffice.Domain.Staff.Tests
{
    public class StaffTest
    {
        private Mock<IConfiguration> mo = new Mock<IConfiguration>();
        private string domain;

        public StaffTest()
        {
            domain = mo.Object["EmailSettings:MyDns"];
        }
        [Fact]
        public void Constructor_ShouldInitializeProperties_WhenValidArguments()
        {
            // Arrange
            var id = new StaffId("D202412345");
            var licenseNumber = new LicenseNumber("12345");
            var specialization = Specializations.SpecializationType.Urology;
            var email = new StaffEmail("D202412345", domain);
            var slots = new List<Slots> { new Slots(DateTime.Now, DateTime.Now.AddHours(1)) };
            var status = new StaffStatus(true);

            // Act
            var staff = new Staff(id, licenseNumber, specialization, email, slots, status);

            // Assert
            Assert.Equal(id, staff.Id);
            Assert.Equal(licenseNumber, staff.LicenseNumber);
            Assert.Equal(specialization, staff.Specialization);
            Assert.Equal(email, staff.Email);
            Assert.Equal(slots, staff.AvailableSlots);
            Assert.Equal(status, staff.Status);
        }

        [Fact]
        public void AddSlot_ShouldAddSlot_WhenSlotIsValid()
        {
            // Arrange
            var id = new StaffId(Guid.NewGuid());
            var licenseNumber = new LicenseNumber("12345");
            var specialization = new Specializations("Cardiology");
            var email = new StaffEmail("test@example.com");
            var slots = new List<Slots> { new Slots(DateTime.Now, DateTime.Now.AddHours(1)) };
            var status = new StaffStatus(true);
            var staff = new Staff(id, licenseNumber, specialization, email, slots, status);
            var newSlot = new Slots(DateTime.Now.AddHours(2), DateTime.Now.AddHours(3));

            // Act
            staff.AddSlot(newSlot);

            // Assert
            Assert.Contains(newSlot, staff.AvailableSlots);
        }

        [Fact]
        public void AddSlot_ShouldThrowException_WhenSlotConflicts()
        {
            // Arrange
            var id = new StaffId(Guid.NewGuid());
            var licenseNumber = new LicenseNumber("12345");
            var specialization = new Specializations("Cardiology");
            var email = new StaffEmail("test@example.com");
            var slots = new List<Slots> { new Slots(DateTime.Now, DateTime.Now.AddHours(1)) };
            var status = new StaffStatus(true);
            var staff = new Staff(id, licenseNumber, specialization, email, slots, status);
            var conflictingSlot = new Slots(DateTime.Now.AddMinutes(30), DateTime.Now.AddHours(1).AddMinutes(30));

            // Act & Assert
            Assert.Throws<BusinessRuleValidationException>(() => staff.AddSlot(conflictingSlot));
        }

        [Fact]
        public void RemoveSlot_ShouldRemoveSlot_WhenSlotExists()
        {
            // Arrange
            var id = new StaffId(Guid.NewGuid());
            var licenseNumber = new LicenseNumber("12345");
            var specialization = new Specializations("Cardiology");
            var email = new StaffEmail("test@example.com");
            var slots = new List<Slots> { new Slots(DateTime.Now, DateTime.Now.AddHours(1)) };
            var status = new StaffStatus(true);
            var staff = new Staff(id, licenseNumber, specialization, email, slots, status);
            var slotToRemove = slots[0];

            // Act
            staff.RemoveSlot(slotToRemove);

            // Assert
            Assert.DoesNotContain(slotToRemove, staff.AvailableSlots);
        }

        [Fact]
        public void Deactivate_ShouldSetStatusToInactive_WhenStatusIsActive()
        {
            // Arrange
            var id = new StaffId(Guid.NewGuid());
            var licenseNumber = new LicenseNumber("12345");
            var specialization = new Specializations("Cardiology");
            var email = new StaffEmail("test@example.com");
            var slots = new List<Slots> { new Slots(DateTime.Now, DateTime.Now.AddHours(1)) };
            var status = new StaffStatus(true);
            var staff = new Staff(id, licenseNumber, specialization, email, slots, status);

            // Act
            staff.Deactivate();

            // Assert
            Assert.False(staff.Status.IsActive);
        }
    }
}