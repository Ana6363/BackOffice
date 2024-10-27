using BackOffice.Application.OperationRequest;
using BackOffice.Domain.OperationType;
using BackOffice.Infrastructure;
using BackOffice.Infrastructure.OperationTypes;
using Microsoft.EntityFrameworkCore;

namespace BackOffice.Application.OperationTypes
{


    public class OperationTypeService
    {
        private readonly IOperationTypeRepository _operationTypeRepository;
        private BackOfficeDbContext _dbContext;

        public OperationTypeService(IOperationTypeRepository operationTypeRepository, BackOfficeDbContext dbContext)
        {
            _operationTypeRepository = operationTypeRepository;
            _dbContext = dbContext;
        }

        public async Task<OperationTypeDataModel> CreateOperationType(OperationTypeDTO operationTypeDTO)
        {
            var existingOperationTypeByName = await _operationTypeRepository.GetByNameAsync(operationTypeDTO.OperationTypeName);
            if (existingOperationTypeByName != null)
            {
                throw new ArgumentException($"Operation Type with name '{operationTypeDTO.OperationTypeName}' already exists.");
            }

            string nextId = Guid.NewGuid().ToString();

            var domainId = new OperationTypeId(nextId);
            var operationType = OperationTypeMapper.ToDomain(operationTypeDTO, domainId);
            Console.WriteLine(operationType.Id.AsString());

            try
            {
                return await _operationTypeRepository.AddAsync(operationType);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to create Operation Type.", ex);
            }
        }

        public async Task<OperationTypeDTO> UpdateAsync(OperationTypeDTO operationTypeDTO)
        {
            var existingOperationType = await _operationTypeRepository.GetByIdAsync(operationTypeDTO.OperationTypeId);
            if (existingOperationType == null)
            {
                throw new ArgumentException("Operation Type does not exist"); // ($"Operation Type with ID '{operationTypeDTO.OperationTypeId}' does not exists.")
            }
            var operationTypeDomain = OperationTypeMapper.FromDataModelToDomain(existingOperationType);
            
            await _operationTypeRepository.UpdateAsync(operationTypeDomain);

            return operationTypeDTO;
            

        }

        public async Task<bool> DeleteOperationTypeAsync(string operationTypeId)
        {
            var existingOperationType = await _operationTypeRepository.GetByIdAsync(operationTypeId);
            if (existingOperationType == null)
            {
                //throw new ArgumentException("Operation Type does not exist"); // ($"Operation Type with ID '{operationTypeDTO.OperationTypeId}' does not exists.")
                return false;
            }
            var operationTypeDomain = OperationTypeMapper.FromDataModelToDomain(existingOperationType);
            
            await _operationTypeRepository.DeleteAsync(operationTypeId);

            return true;
            
        }


        public async Task<OperationTypeDataModel> GetByIdAsync(OperationTypeId operationTypeId)
        {
            //var existingOperationType = await _operationTypeRepository.GetByIdAsync(operationTypeId.AsString());
            
            if (operationTypeId == null)
            {
                throw new ArgumentNullException(nameof(operationTypeId), "OperationType ID cannot be null.");
            }
            return await _operationTypeRepository.GetByIdAsync(operationTypeId.AsString());
        }

        public async Task<List<OperationTypeDataModel>> GetAllAsync()
        {
            return await _operationTypeRepository.GetAllAsync();

        }


    }
}