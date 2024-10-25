using System;
using BackOffice.Domain.Patients;
using BackOffice.Domain.Shared;
using BackOffice.Domain.Users;
using Moq;
using Xunit;

namespace BackOffice.DomainTests.Test.Patients
{
    public class PatientTest
    {
        [Fact]
        public void CreatePatient_WithValidParameters_ShouldCreatePatient()
        {
            // Arrange
            var recordNumber = new Mock<RecordNumber>("12345");
            var userId = new Mock<UserId>("user-12345");
            var dateOfBirth = new Mock<DateOfBirth>(new DateTime(1990, 1, 1));
            var emergencyContact = new Mock<PhoneNumber>(1234567890);
            var gender = new Mock<Gender>(Gender.GenderType.MALE);

            var mockPatientRepository = new Mock<IPatientRepository>();
            var mockUserRepository = new Mock<IUserRepository>();

            // Act
            var patient = new Patient(recordNumber.Object, userId.Object, dateOfBirth.Object, emergencyContact.Object, gender.Object);

            // Assert
            Assert.NotNull(patient);
            Assert.Equal(recordNumber.Object, patient.RecordNumber);
            Assert.Equal(userId.Object, patient.UserId);
            Assert.Equal(dateOfBirth.Object, patient.DateOfBirth);
            Assert.Equal(emergencyContact.Object, patient.EmergencyContact);
            Assert.Equal(gender.Object, patient.Gender);
        }

        [Fact]
        public void CreatePatient_WithNullRecordNumber_ShouldThrowException()
        {
            // Arrange
            var userId = new Mock<UserId>("user-12345");
            var dateOfBirth = new Mock<DateOfBirth>(new DateTime(1990, 1, 1));
            var emergencyContact = new Mock<PhoneNumber>(1234567890);
            var gender = new Mock<Gender>(Gender.GenderType.MALE);

            var mockPatientRepository = new Mock<IPatientRepository>();
            var mockUserRepository = new Mock<IUserRepository>();

            // Act & Assert
            Assert.Throws<BusinessRuleValidationException>(() => new Patient(null!, userId.Object, dateOfBirth.Object, emergencyContact.Object, gender.Object));
        }

        [Fact]
        public void CreatePatient_WithNullUserId_ShouldThrowException()
        {
            // Arrange
            var recordNumber = new Mock<RecordNumber>("12345");
            var dateOfBirth = new Mock<DateOfBirth>(new DateTime(1990, 1, 1));
            var emergencyContact = new Mock<PhoneNumber>(1234567890);
            var gender = new Mock<Gender>(Gender.GenderType.MALE);

            var mockPatientRepository = new Mock<IPatientRepository>();
            var mockUserRepository = new Mock<IUserRepository>();

            // Act & Assert
            Assert.Throws<BusinessRuleValidationException>(() => new Patient(recordNumber.Object, null!, dateOfBirth.Object, emergencyContact.Object, gender.Object));
        }

        [Fact]
        public void CreatePatient_WithNullDateOfBirth_ShouldThrowException()
        {
            // Arrange
            var recordNumber = new Mock<RecordNumber>("12345");
            var userId = new Mock<UserId>("user-12345");
            var emergencyContact = new Mock<PhoneNumber>(1234567890);
            var gender = new Mock<Gender>(Gender.GenderType.MALE);

            var mockPatientRepository = new Mock<IPatientRepository>();
            var mockUserRepository = new Mock<IUserRepository>();

            // Act & Assert
            Assert.Throws<BusinessRuleValidationException>(() => new Patient(recordNumber.Object, userId.Object, null!, emergencyContact.Object, gender.Object));
        }

        [Fact]
        public void CreatePatient_WithNullEmergencyContact_ShouldThrowException()
        {
            // Arrange
            var recordNumber = new Mock<RecordNumber>("12345");
            var userId = new Mock<UserId>("user-12345");
            var dateOfBirth = new Mock<DateOfBirth>(new DateTime(1990, 1, 1));
            var gender = new Mock<Gender>(Gender.GenderType.MALE);

            var mockPatientRepository = new Mock<IPatientRepository>();
            var mockUserRepository = new Mock<IUserRepository>();

            // Act & Assert
            Assert.Throws<BusinessRuleValidationException>(() => new Patient(recordNumber.Object, userId.Object, dateOfBirth.Object, null!, gender.Object));
        }

        [Fact]
        public void CreatePatient_WithNullGender_ShouldThrowException()
        {
            // Arrange
            var recordNumber = new Mock<RecordNumber>("12345");
            var userId = new Mock<UserId>("user-12345");
            var dateOfBirth = new Mock<DateOfBirth>(new DateTime(1990, 1, 1));
            var emergencyContact = new Mock<PhoneNumber>(1234567890);

            var mockPatientRepository = new Mock<IPatientRepository>();
            var mockUserRepository = new Mock<IUserRepository>();

            // Act & Assert
            Assert.Throws<BusinessRuleValidationException>(() => new Patient(recordNumber.Object, userId.Object, dateOfBirth.Object, emergencyContact.Object, null!));
        }
    }
}