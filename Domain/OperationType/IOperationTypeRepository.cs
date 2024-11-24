using BackOffice.Application.OperationTypes;
using BackOffice.Domain.Staff;
using BackOffice.Infrastructure.OperationTypes;

namespace BackOffice.Domain.OperationType
{

    public interface IOperationTypeRepository
    {
        Task<OperationTypeDataModel> AddAsync (OperationTypeDataModel operationType);
        Task<OperationTypeDataModel> GetByIdAsync (string id); // changed from operationtypeid to string
        Task<List<OperationTypeDataModel>> GetAllAsync();
        Task UpdateAsync (OperationTypeDataModel operationType);
        Task DeleteAsync (string id);
        Task<OperationTypeDataModel> GetByNameAsync (string name);
        Task<string> GetMaxOperationTypeIdAsync();
    }
}