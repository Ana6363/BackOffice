/* using BackOffice.Application.OperationTypes;
using BackOffice.Domain.OperationType;
using BackOffice.Domain.Shared;
using BackOffice.Domain.Staff;
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

        [Fact]
        public async Task createOperationTypeTest()
        {
            
            // create dto
            var id = Guid.NewGuid().ToString();
            var operationTypeDTO = new OperationTypeDTO
            {
                OperationTypeId = id,
                OperationTypeName = "Cardiology",
                OperationTime = 2.5f,
                Specializations = new List<SpecializationDTO>
                {
                    new SpecializationDTO
                    {
                        Name = "Cardiology"
                    }
                } 
            };
            // create operation type from dto data
            var operationType = new OperationType(
            new OperationTypeId(operationTypeDTO.OperationTypeId),
            new OperationTypeName(operationTypeDTO.OperationTypeName),
            new OperationTime(operationTypeDTO.OperationTime),
            operationTypeDTO.Specializations.Select(s => Specializations.FromString(s.Name)).ToList() // Convert to Specializations
            );


            var _operationTypeRepositoryMock = new Mock<IOperationTypeRepository>();

            
            // Mock AddAsync to return the OperationTypeDataModel
            _operationTypeRepositoryMock
                .Setup(repo => repo.AddAsync(It.IsAny<OperationType>()))
                .ReturnsAsync(new OperationTypeDataModel
                {
                    OperationTypeId = operationType.Id.Value,
                    OperationTypeName = operationType.OperationTypeName.Name,
                    OperationTime = operationType.OperationTime.AsFloat(),
                    Specializations = operationType.Specializations.Select(s => new SpecializationDataModel
                    {
                        SpecializationId = Guid.NewGuid().ToString(), 
                        Name = s.ToString(),
                        OperationTypeId = operationType.Id.Value
                    }).ToList()
                });


            var options = new DbContextOptionsBuilder<BackOfficeDbContext>()
                .Options;

            using var context = new BackOfficeDbContext(options);

            
            var service = new OperationTypeService(_operationTypeRepositoryMock.Object, context);
            
            // Act
            var result = await service.CreateOperationType(operationTypeDTO);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(operationType.OperationTypeName.Name, result.OperationTypeName);
            Assert.Equal(operationType.OperationTime.AsFloat(), result.OperationTime);
            Assert.Equal(operationType.Id.Value, result.OperationTypeId);
            Assert.Single(result.Specializations); // Check if it has one Specialization (Cardiology)
            Assert.Equal("Cardiology", result.Specializations[0].Name); // Check Specialization


            
        }










    }
} */