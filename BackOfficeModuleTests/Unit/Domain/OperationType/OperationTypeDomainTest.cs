using BackOffice.Domain.OperationType;
using BackOffice.Domain.Shared;
using BackOffice.Domain.Staff;
using Xunit;

namespace BackOffice.BackOfficeModuleTest.Unit.OperationTypeServiceTest
{

    public class OperationTypeDomainTest
    {
        [Fact]
        public void CreateOperationTypeNameInsuccessEmpty()
        {
        var exception = Assert.Throws<BusinessRuleValidationException>(() => new OperationTypeName(string.Empty));
        Assert.Equal("Operation Type name connot be null or empty", exception.Message);


        

        }
        [Fact]
        public void CreateOperationTypeNameInsuccessNotString()
        {
        var exception = Assert.Throws<BusinessRuleValidationException>(() => new OperationTypeName("243"));
        Assert.Equal("Operation Type name must be a String", exception.Message);

        }

        [Fact]
        public void CreateOperationTypeNameSuccess()
        {
            var validName = "Cardiology";

            var opName = new OperationTypeName("Cardiology");
            Assert.Equal(validName, opName.Name);
            
        }


        [Fact]
        public void CreateOperationTime_NegativeValue()
        {
            // Arrange & Act
            var exception = Assert.Throws<BusinessRuleValidationException>(() => new OperationTime(-1));

            // Assert
            Assert.Equal("Duration of operations cannot be negative.", exception.Message);
    
        }

        [Fact]
        public void CreateOperationTime_ExceedsMaxDuration()
        {
            // Arrange & Act
            var exception = Assert.Throws<BusinessRuleValidationException>(() => new OperationTime(10));

            // Assert
            Assert.Equal("Duration of operations cannot exceed 8 hours", exception.Message);
        }

        [Fact]
        public void CreateOperationTime_Success()
        {
            // Arrange & Act
            var validDuration = 5;

            var opDuration = new OperationTime(validDuration);

            // Assert
            Assert.Equal(validDuration, opDuration.time);
            
        }

        [Fact]
        public void FromString_ValidSpecialization_ReturnsSpecialization()
        {
            // Arrange
            string specializationName = "Cardiology";

            // Act
            var specialization = Specializations.FromString(specializationName);

            // Assert
            Assert.NotNull(specialization);
            Assert.Equal(Specializations.SpecializationType.Cardiology, specialization.Value);
        }

        [Fact]
        public void FromString_InvalidSpecialization_ThrowsArgumentException()
        {
            // Arrange
            string invalidSpecializationName = "InvalidSpecialization";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => Specializations.FromString(invalidSpecializationName));
            Assert.Equal("Invalid specialization: InvalidSpecialization", exception.Message);
        }

        [Fact]
        public void FromEnum_ValidEnum_ReturnsSpecialization()
        {
            // Arrange
            var specializationType = Specializations.SpecializationType.Orthopedics;

            // Act
            var specialization = Specializations.FromEnum(specializationType);

            // Assert
            Assert.NotNull(specialization);
            Assert.Equal(specializationType, specialization.Value);
        }

        [Fact]
        public void Equals_SameValue_ReturnsTrue()
        {
            // Arrange
            var specialization1 = Specializations.FromString("Neurology");
            var specialization2 = Specializations.FromString("Neurology");

            // Act
            var areEqual = specialization1.Equals(specialization2);

            // Assert
            Assert.True(areEqual);
        }

        [Fact]
        public void Equals_DifferentValue_ReturnsFalse()
        {
            // Arrange
            var specialization1 = Specializations.FromString("Pediatrics");
            var specialization2 = Specializations.FromString("Cardiology");

            // Act
            var areEqual = specialization1.Equals(specialization2);

            // Assert
            Assert.False(areEqual);
        }

        


    }
}