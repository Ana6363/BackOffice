using System;
using System.Collections.Generic;
using BackOffice.Domain.Shared;
using BackOffice.Domain.Staff;
using BackOffice.Infrastructure.Staff;
using Moq;
using Xunit;

namespace BackOffice.DomainTests.Staff.Tests
{
    public class StaffTest
    {
        private Mock<IStaffRepository> mockRepository;
        private Mock<IConfiguration> mockConfiguration;

        public StaffTest()
        {
            mockRepository = new Mock<IStaffRepository>();
            mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["SomeSetting"]).Returns("SomeValue");
        }

        [Fact]
        public void Constructor_ShouldInitializeProperties_WhenValidArguments()
        {
            // Arrange
            var id = new Mock<StaffId>("some-id");
            var licenseNumber = new Mock<LicenseNumber>("12345");
            var specialization = Specializations.FromString("Cardiology");
            var email = new Mock<StaffEmail>("test@example.com", mockConfiguration.Object);
            var slots = new List<Slots> { new Slots(DateTime.Now, DateTime.Now.AddHours(1)) };
            var status = new Mock<StaffStatus>(true);

            // Act
            var staff = new BackOffice.Domain.Staff.Staff(id.Object, licenseNumber.Object, specialization, email.Object, slots, status.Object);

            // Assert
            Assert.Equal(id.Object, staff.Id);
            Assert.Equal(licenseNumber.Object, staff.LicenseNumber);
            Assert.Equal(specialization, staff.Specialization);
            Assert.Equal(email.Object, staff.Email);
            Assert.Equal(slots, staff.AvailableSlots);
            Assert.Equal(status.Object, staff.Status);
        }

        [Fact]
        public void Constructor_ShouldThrowException_WhenIdIsNull()
        {
            // Arrange
            StaffId id = null;
            var licenseNumber = new Mock<LicenseNumber>("12345");
            var specialization = Specializations.FromString("Cardiology");
            var email = new Mock<StaffEmail>("test@example.com", mockConfiguration.Object);
            var slots = new List<Slots> { new Slots(DateTime.Now, DateTime.Now.AddHours(1)) };
            var status = new Mock<StaffStatus>(true);

            // Act & Assert
            Assert.Throws<BusinessRuleValidationException>(() => new BackOffice.Domain.Staff.Staff(id, licenseNumber.Object, specialization, email.Object, slots, status.Object));
        }

        [Fact]
        public void AddSlot_ShouldAddSlot_WhenSlotIsValid()
        {
            // Arrange
            var staff = CreateValidStaff();
            var newSlot = new Slots(DateTime.Now.AddHours(2), DateTime.Now.AddHours(3));

            // Act
            staff.AddSlot(newSlot);

            // Assert
            Assert.Contains(newSlot, staff.AvailableSlots);
        }

        [Fact]
        public void AddSlot_ShouldThrowException_WhenSlotIsNull()
        {
            // Arrange
            var staff = CreateValidStaff();
            Slots newSlot = null;

            // Act & Assert
            Assert.Throws<BusinessRuleValidationException>(() => staff.AddSlot(newSlot));
        }

        [Fact]
        public async Task AddSlot_ShouldThrowException_WhenSlotConflicts()
        {
            // Arrange
            var staff = CreateValidStaff();
            var conflictingSlot = new Slots(DateTime.Now, DateTime.Now.AddHours(1));

            // Act & Assert
            await Assert.ThrowsAsync<BusinessRuleValidationException>(() => Task.Run(() => staff.AddSlot(conflictingSlot)));
        }

        [Fact]
        public void RemoveSlot_ShouldRemoveSlot_WhenSlotExists()
        {
            // Arrange
            var staff = CreateValidStaff();
            var slotToRemove = staff.AvailableSlots[0];

            // Act
            staff.RemoveSlot(slotToRemove);

            // Assert
            Assert.DoesNotContain(slotToRemove, staff.AvailableSlots);
        }

        [Fact]
        public void Deactivate_ShouldSetStatusToInactive_WhenStatusIsActive()
        {
            // Arrange
            var staff = CreateValidStaff();

            // Act
            staff.Deactivate();

            // Assert
            Assert.False(staff.Status.IsActive);
        }

        [Fact]
        public void Constructor_ShouldThrowException_WhenLicenseNumberIsNull()
        {
            // Arrange
            var id = new Mock<StaffId>("some-id");
            LicenseNumber licenseNumber = null;
            var specialization = Specializations.FromString("Cardiology");
            var email = new Mock<StaffEmail>("test@example.com", mockConfiguration.Object);
            var slots = new List<Slots> { new Slots(DateTime.Now, DateTime.Now.AddHours(1)) };
            var status = new Mock<StaffStatus>(true);

            // Act & Assert
            Assert.Throws<BusinessRuleValidationException>(() => new BackOffice.Domain.Staff.Staff(id.Object, licenseNumber, specialization, email.Object, slots, status.Object));
        }

        [Fact]
        public void Constructor_ShouldThrowException_WhenSpecializationIsNull()
        {
            // Arrange
            var id = new Mock<StaffId>("some-id");
            var licenseNumber = new Mock<LicenseNumber>("12345");
            Specializations specialization = null;
            var email = new Mock<StaffEmail>("test@example.com", mockConfiguration.Object);
            var slots = new List<Slots> { new Slots(DateTime.Now, DateTime.Now.AddHours(1)) };
            var status = new Mock<StaffStatus>(true);

            // Act & Assert
            Assert.Throws<BusinessRuleValidationException>(() => new BackOffice.Domain.Staff.Staff(id.Object, licenseNumber.Object, specialization, email.Object, slots, status.Object));
        }

        [Fact]
        public void Constructor_ShouldThrowException_WhenEmailIsNull()
        {
            // Arrange
            var id = new Mock<StaffId>("some-id");
            var licenseNumber = new Mock<LicenseNumber>("12345");
            var specialization = Specializations.FromString("Cardiology");
            StaffEmail email = null;
            var slots = new List<Slots> { new Slots(DateTime.Now, DateTime.Now.AddHours(1)) };
            var status = new Mock<StaffStatus>(true);

            // Act & Assert
            Assert.Throws<BusinessRuleValidationException>(() => new BackOffice.Domain.Staff.Staff(id.Object, licenseNumber.Object, specialization, email, slots, status.Object));
        }

        [Fact]
        public void Constructor_ShouldThrowException_WhenSlotsIsNull()
        {
            // Arrange
            var id = new Mock<StaffId>("some-id");
            var licenseNumber = new Mock<LicenseNumber>("12345");
            var specialization = Specializations.FromString("Cardiology");
            var email = new Mock<StaffEmail>("test@example.com", mockConfiguration.Object);
            List<Slots> slots = null;
            var status = new Mock<StaffStatus>(true);

            // Act & Assert
            Assert.Throws<BusinessRuleValidationException>(() => new BackOffice.Domain.Staff.Staff(id.Object, licenseNumber.Object, specialization, email.Object, slots, status.Object));
        }

        [Fact]
        public void Constructor_ShouldThrowException_WhenSlotsIsEmpty()
        {
            // Arrange
            var id = new Mock<StaffId>("some-id");
            var licenseNumber = new Mock<LicenseNumber>("12345");
            var specialization = Specializations.FromString("Cardiology");
            var email = new Mock<StaffEmail>("test@example.com", mockConfiguration.Object);
            var slots = new List<Slots>();
            var status = new Mock<StaffStatus>(true);

            // Act & Assert
            Assert.Throws<BusinessRuleValidationException>(() => new BackOffice.Domain.Staff.Staff(id.Object, licenseNumber.Object, specialization, email.Object, slots, status.Object));
        }

        [Fact]
        public void Constructor_ShouldThrowException_WhenStatusIsNull()
        {
            // Arrange
            var id = new Mock<StaffId>("some-id");
            var licenseNumber = new Mock<LicenseNumber>("12345");
            var specialization = Specializations.FromString("Cardiology");
            var email = new Mock<StaffEmail>("test@example.com", mockConfiguration.Object);
            var slots = new List<Slots> { new Slots(DateTime.Now, DateTime.Now.AddHours(1)) };
            StaffStatus status = null;

            // Act & Assert
            Assert.Throws<BusinessRuleValidationException>(() => new BackOffice.Domain.Staff.Staff(id.Object, licenseNumber.Object, specialization, email.Object, slots, status));
        }

        private BackOffice.Domain.Staff.Staff CreateValidStaff()
        {
            var id = new Mock<StaffId>("some-id");
            var licenseNumber = new Mock<LicenseNumber>("12345");
            var specialization = Specializations.FromString("Cardiology");
            var email = new Mock<StaffEmail>("test@example.com", mockConfiguration.Object);
            var slots = new List<Slots> { new Slots(DateTime.Now, DateTime.Now.AddHours(1)) };
            var status = new Mock<StaffStatus>(true);

            return new BackOffice.Domain.Staff.Staff(id.Object, licenseNumber.Object, specialization, email.Object, slots, status.Object);
        }
    }
}
