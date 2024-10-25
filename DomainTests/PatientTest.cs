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
            var recordNumber = new RecordNumber("12345");
            var userId = new UserId(Guid.NewGuid());
            var dateOfBirth = new DateOfBirth(new DateTime(1990, 1, 1));
            var emergencyContact = new PhoneNumber("123-456-7890");
            var gender = new Gender("Male");

            var mockPatientRepository = new Mock<IPatientRepository>();
            var mockUserRepository = new Mock<IUserRepository>();

            // Act
            var patient = new Patient(recordNumber, userId, dateOfBirth, emergencyContact, gender);

            // Assert
            Assert.NotNull(patient);
            Assert.Equal(recordNumber, patient.RecordNumber);
            Assert.Equal(userId, patient.UserId);
            Assert.Equal(dateOfBirth, patient.DateOfBirth);
            Assert.Equal(emergencyContact, patient.EmergencyContact);
            Assert.Equal(gender, patient.Gender);
        }

        [Fact]
        public void CreatePatient_WithNullRecordNumber_ShouldThrowException()
        {
            // Arrange
            var userId = new UserId(Guid.NewGuid());
            var dateOfBirth = new DateOfBirth(new DateTime(1990, 1, 1));
            var emergencyContact = new PhoneNumber("123-456-7890");
            var gender = new Gender("Male");

            // Act & Assert
            Assert.Throws<BusinessRuleValidationException>(() => new Patient(null, userId, dateOfBirth, emergencyContact, gender));
        }

        [Fact]
        public void CreatePatient_WithNullUserId_ShouldThrowException()
        {
            // Arrange
            var recordNumber = new RecordNumber("12345");
            var dateOfBirth = new DateOfBirth(new DateTime(1990, 1, 1));
            var emergencyContact = new PhoneNumber("123-456-7890");
            var gender = new Gender("Male");

            // Act & Assert
            Assert.Throws<BusinessRuleValidationException>(() => new Patient(recordNumber, null, dateOfBirth, emergencyContact, gender));
        }

        [Fact]
        public void CreatePatient_WithNullDateOfBirth_ShouldThrowException()
        {
            // Arrange
            var recordNumber = new RecordNumber("12345");
            var userId = new UserId(Guid.NewGuid());
            var emergencyContact = new PhoneNumber("123-456-7890");
            var gender = new Gender("Male");

            // Act & Assert
            Assert.Throws<BusinessRuleValidationException>(() => new Patient(recordNumber, userId, null, emergencyContact, gender));
        }

        [Fact]
        public void CreatePatient_WithNullEmergencyContact_ShouldThrowException()
        {
            // Arrange
            var recordNumber = new RecordNumber("12345");
            var userId = new UserId(Guid.NewGuid());
            var dateOfBirth = new DateOfBirth(new DateTime(1990, 1, 1));
            var gender = new Gender("Male");

            // Act & Assert
            Assert.Throws<BusinessRuleValidationException>(() => new Patient(recordNumber, userId, dateOfBirth, null, gender));
        }

        [Fact]
        public void CreatePatient_WithNullGender_ShouldThrowException()
        {
            // Arrange
            var recordNumber = new RecordNumber("12345");
            var userId = new UserId(Guid.NewGuid());
            var dateOfBirth = new DateOfBirth(new DateTime(1990, 1, 1));
            var emergencyContact = new PhoneNumber("123-456-7890");

            // Act & Assert
            Assert.Throws<BusinessRuleValidationException>(() => new Patient(recordNumber, userId, dateOfBirth, emergencyContact, null));
        }
    }
}