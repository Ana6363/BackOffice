using BackOffice.Application.OperationTypes;
using BackOffice.Domain.OperationType;
using BackOffice.Domain.Shared;
using BackOffice.Infrastructure;
using BackOffice.Infrastructure.OperationTypes;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace BackOffice.BackOfficeModuleTest.Unit.OperationTypeServiceTest
{

    public class OperationTypeServiceTest
    {
        private string id = "testID";
        private string name = "OperationTypeName";
        private float time = 4;
        private List<string> specializations = new List<string> { "Neurology", "Dermatology" }; // Exemplo de especializações


        [Fact]
        public void OperationTypeServiceConstructor_ShouldInitializeCorrectly()
        {
            // Arrange
            var _operationTypeRepositoryMock = new Mock<IOperationTypeRepository>();
            //var _dbContextMock = new Mock<BackOfficeDbContext>(); // Mock do DbContext

            var options = new DbContextOptionsBuilder<BackOfficeDbContext>()
                .Options;

            using var context = new BackOfficeDbContext(options);
            // Act
            var service = new OperationTypeService(_operationTypeRepositoryMock.Object, context);

            // Assert
            Assert.NotNull(service); 
        }

        
        private List<OperationTypeDataModel> createdOperationTypes()
        {
            return new List<OperationTypeDataModel>
            {
                new OperationTypeDataModel
                {
                    OperationTypeId = "1",
                    OperationTypeName = "Operation Type 1",
                    OperationTime = 4.5f,
                    Specializations = new List<SpecializationDataModel>
                    {
                        new SpecializationDataModel { SpecializationId = "1", Name = "Neurology", OperationTypeId = "1" },
                        new SpecializationDataModel { SpecializationId = "2", Name = "Dermatology", OperationTypeId = "1" }
                    }
                },
                new OperationTypeDataModel
                {
                    OperationTypeId = "2",
                    OperationTypeName = "Operation Type 2",
                    OperationTime = 3.0f,
                    Specializations = new List<SpecializationDataModel>
                    {
                        new SpecializationDataModel { SpecializationId = "3", Name = "Neurology", OperationTypeId = "2" }
                    }
                }
            };
        }

    

        [Fact]
        public async Task getOperationTypesTest(){

            var _operationTypeRepositoryMock = new Mock<IOperationTypeRepository>();

            _operationTypeRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(createdOperationTypes());
            
            var options = new DbContextOptionsBuilder<BackOfficeDbContext>()
                .Options;

            using var context = new BackOfficeDbContext(options);

            
            // Act
            var service = new OperationTypeService(_operationTypeRepositoryMock.Object, context);

            var expectedList = await service.GetAllAsync();

            var resultList = createdOperationTypes();

            // Verifica se as contagens são iguais
            Assert.Equal(resultList.Count, expectedList.Count); 
            // Verifica se a contagem das especializações é igual
            Assert.Equal(resultList[0].Specializations.Count, expectedList[0].Specializations.Count); 
    


        }








    }
}