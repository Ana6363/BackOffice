using BackOffice.Domain.OperationRequest;
using BackOffice.Infraestructure.OperationRequest;

namespace BackOffice.Domain.OperationRequest
{
    public interface IOperationRequestRepository
    {
        Task<OperationRequestDataModel> AddAsync(OperationRequest request);
        Task<OperationRequestDataModel> GetByIdAsync(RequestId id);
        Task<List<OperationRequestDataModel>> GetAllAsync();
        Task UpdateAsync(OperationRequest request);
        Task DeleteAsync(RequestId id);
    }
}
