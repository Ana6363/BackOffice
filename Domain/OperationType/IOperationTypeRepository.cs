using BackOffice.Domain.Staff;
using BackOffice.Infrastructure.OperationTypes;

namespace BackOffice.Domain.OperationType
{

    public interface IOperationTypeRepository
    {
        Task<OperationTypeDataModel> AddAsync (OperationType operationType);
        Task<OperationTypeDataModel> GetByIdAsync (OperationTypeId id);
        Task<List<OperationTypeDataModel>> GetAllAsync();
        Task UpdateAsync (OperationType operationType);
        Task DeleteAsync (OperationTypeId id);
    }
}