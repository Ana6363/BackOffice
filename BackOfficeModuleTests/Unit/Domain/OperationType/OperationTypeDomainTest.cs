using BackOffice.Domain.OperationType;
using BackOffice.Domain.Shared;
using BackOffice.Domain.Specialization;
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
            var specialization = new Specializations("Urology");

            // Assert
            Assert.NotNull(specialization);
            Assert.Equal("Cardiology", specialization.Value);
        }

        [Fact]
        public void FromString_InvalidSpecialization_ThrowsArgumentException()
        {
            // Arrange
            string invalidSpecializationName = "InvalidSpecialization";

        }


        [Fact]
        public void Equals_SameValue_ReturnsTrue()
        {
            // Arrange
            var specialization1 = new Specializations("Neurology");
            var specialization2 = new Specializations("Neurology");

            // Act
            var areEqual = specialization1.Equals(specialization2);

            // Assert
            Assert.True(areEqual);
        }

        [Fact]
        public void Equals_DifferentValue_ReturnsFalse()
        {
            // Arrange
            var specialization1 = new Specializations("Pediatrics");
            var specialization2 = new Specializations("Cardiology");

            // Act
            var areEqual = specialization1.Equals(specialization2);

            // Assert
            Assert.False(areEqual);
        }

        


    }
}