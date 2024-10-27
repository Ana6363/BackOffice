using BackOffice.Domain.OperationType;
using BackOffice.Domain.Shared;
using Xunit;

namespace BackOffice.BackOfficeModuleTest.Unit.OperationTypeServiceTest
{

    public class OperationTypeDomainTest
    {
        [Fact]
        public void CreateOperationTypeNameInsuccess()
        {
        var exception = Assert.Throws<BusinessRuleValidationException>(() => new OperationTypeName(string.Empty));
        Assert.Equal("Operation Type name connot be null or empty", exception.Message);


        

        }

        [Fact]
        public void CreateOperationTypeNameSuccess()
        {
            var validName = "Cardiology";

            var opName = new OperationTypeName("Cardiology");
            Assert.Equal(validName, opName.Name);
            
        }



    }
}