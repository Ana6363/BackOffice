using BackOffice.Domain.OperationRequest;
using Microsoft.EntityFrameworkCore;
using BackOffice.Infrastructure;

namespace BackOffice.Infraestructure.OperationRequest
{
    public class OperationRequestRepository : IOperationRequestRepository
    {
        private readonly BackOfficeDbContext _context;

        public OperationRequestRepository(BackOfficeDbContext context)
        {
            _context = context;
        }

        public async Task<OperationRequestDataModel> AddAsync(Domain.OperationRequest.OperationRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request), "Request cannot be null.");
            }
            var operationRequestDataModel = new OperationRequestDataModel
            {
                RequestId = Guid.Parse(request.RequestId.AsString()),
                DeadLine = request.DeadLine.Value,
                Priority = request.Priority.Value.ToString(),
                RecordNumber = request.Patient.AsString(),
                StaffId = request.StaffId.AsString(),
                Status = request.Status.Value.ToString(),
                OperationType = request.OperationTypeId.Value.ToString()
            };

            await _context.OperationRequests.AddAsync(operationRequestDataModel);
            await _context.SaveChangesAsync();

            return operationRequestDataModel;
        }

        public async Task<OperationRequestDataModel?> GetByIdAsync(RequestId id)
        {
            var requestIdString = id.AsString();

            return await _context.OperationRequests
                .FirstOrDefaultAsync(p => p.RequestId.ToString() == requestIdString);
        }

        public async Task<List<OperationRequestDataModel>> GetAllAsync()
        {
            return await _context.OperationRequests.ToListAsync();
        }

        public async Task UpdateAsync(Domain.OperationRequest.OperationRequest request)
        {
            var operationRequestDataModel = await GetByIdAsync(request.RequestId);
            if (operationRequestDataModel == null)
            {
                throw new Exception("Request not found."); // Handle not found case
            }

            operationRequestDataModel.DeadLine = request.DeadLine.Value;
            operationRequestDataModel.Priority = request.Priority.Value.ToString();
            operationRequestDataModel.RecordNumber = request.Patient.AsString();
            operationRequestDataModel.StaffId = request.StaffId.AsString();
            operationRequestDataModel.Status = request.Status.ToString();
            operationRequestDataModel.OperationType = request.OperationTypeId.Value.ToString();

            _context.OperationRequests.Update(operationRequestDataModel);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(RequestId id)
        {
            var operationRequestDataModel = await GetByIdAsync(id);
            if (operationRequestDataModel != null)
            {
                _context.OperationRequests.Remove(operationRequestDataModel);
                await _context.SaveChangesAsync();
            }
        }
    }
    
    }

