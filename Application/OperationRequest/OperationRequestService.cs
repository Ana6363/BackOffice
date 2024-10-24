using BackOffice.Domain.Appointement;
using BackOffice.Domain.OperationRequest;
using BackOffice.Domain.Shared;
using BackOffice.Infraestructure.OperationRequest;
using BackOffice.Infrastructure;
using BackOffice.Infrastructure.Staff;
using Microsoft.EntityFrameworkCore;

namespace BackOffice.Application.OperationRequest
{
    public class OperationRequestService
    {
        private readonly IAppointementRepository _appointementRepository;
        private readonly BackOfficeDbContext _context;
        private readonly IOperationRequestRepository _operationRequestRepository;
        private readonly IStaffRepository _staffRepository;
        private readonly IUnitOfWork _unitOfWork;

        public OperationRequestService(IOperationRequestRepository operationRequestRepository,
            IAppointementRepository appointementRepository, IUnitOfWork unitOfWork, BackOfficeDbContext context)
        {
            _operationRequestRepository = operationRequestRepository;
            _appointementRepository = appointementRepository;
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public async Task<OperationRequestDataModel> CreateOperationRequestAsync(OperationRequestDto operationRequest)
        {

            var requestDto = new OperationRequestDto(
                Guid.NewGuid(),
                operationRequest.DeadLine,
                operationRequest.Priority,
                operationRequest.RecordNumber,
                operationRequest.StaffId,
                Status.StatusType.PENDING.ToString(),
                operationRequest.OperationTypeName
            );

            Console.WriteLine($"RecordNumber in DTO: {requestDto.RecordNumber}"); // Before conversion

            var request = OperationRequestMapper.ToDomain(requestDto);
            Console.WriteLine($"RecordNumber in Domain: {request.Patient.AsString()}"); // After conversion
            if (request == null)
            {
                Console.WriteLine("Error mapping");
                throw new Exception("Request is null");
            }

            var staff = await _staffRepository.GetByStaffIdAsync(operationRequest.StaffId);
            if (staff.Specialization != request.OperationTypeId.Name) { 
                Console.WriteLine("Staff specialization does not match operation type");
                throw new Exception("Staff specialization does not match operation type");
            }
            try
            {
                return await _operationRequestRepository.AddAsync(request);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error adding to repo: {e.Message}");
                throw;
            }

        }

        public async Task<IEnumerable<OperationRequestDataModel>> GetFilteredRequestAsync(FilteredRequestDto filteredRequest)
        {
            var query = from request in _context.OperationRequests
                        join patient in _context.Patients on request.RecordNumber equals patient.RecordNumber
                        join user in _context.Users on patient.UserId equals user.Id
                        select new { request, patient, user };

            if (!string.IsNullOrWhiteSpace(filteredRequest.PatientName))
            {
                query = query.Where(p => p.user.FullName.Contains(filteredRequest.PatientName));
            }

            if(!string.IsNullOrWhiteSpace(filteredRequest.Priority))
            {
                query = query.Where(p => p.request.Priority.Contains(filteredRequest.Priority));
            }

            if(!string.IsNullOrWhiteSpace(filteredRequest.Status)) 
            {
                query = query.Where(p => p.request.Status.Contains(filteredRequest.Status));
            }

            if(!string.IsNullOrWhiteSpace(filteredRequest.TypeId))
            {
                query = query.Where(p => p.request.OperationType.Contains(filteredRequest.TypeId));
            }

            var result = await query.Select(r => r.request).ToListAsync();
            return result;


        }

    }
}
