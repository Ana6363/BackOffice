using BackOffice.Application.Logs;
using BackOffice.Domain.Appointement;
using BackOffice.Domain.Logs;
using BackOffice.Domain.OperationRequest;
using BackOffice.Domain.Shared;
using BackOffice.Domain.Users;
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
        private readonly IUserRepository _userRepository;
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
            if(request == null)
            {
                Console.WriteLine("Error mapping");
                throw new Exception("Request is null");
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

            var result = await query.Select(r => r.request).ToListAsync();
            return result;


        }

    public async Task<OperationRequestDto> UpdateAsync(OperationRequestDto updatedRequestDto)
{
    var existingRequest = await _context.OperationRequests
        .FirstOrDefaultAsync(r => r.RequestId == updatedRequestDto.RequestId);

    if (existingRequest == null)
    {
        throw new Exception("Operation request not found.");
    }

    var doctor =  await _context.Staff
        .FirstOrDefaultAsync(s => s.StaffId == updatedRequestDto.StaffId);
    
    var doctorEmail = await _context.Users
        .Where(u => u.Id == doctor.Email)
        .Select(u => u.Id)
        .FirstOrDefaultAsync();

    if (existingRequest.StaffId != doctorEmail)
    {
        throw new Exception("Only the requesting doctor can update this operation request.");
    }

    bool isUpdated = false;

    if (updatedRequestDto.DeadLine != existingRequest.DeadLine)
    {
        existingRequest.DeadLine = updatedRequestDto.DeadLine;
        isUpdated = true;
    }

    if (updatedRequestDto.Priority != existingRequest.Priority)
    {
        existingRequest.Priority = updatedRequestDto.Priority;
        isUpdated = true;
    }

    if (isUpdated)
    {
        await LogUpdateOperation(doctorEmail, updatedRequestDto);
        await _context.SaveChangesAsync();
    }

    var updatedRequest= OperationRequestMapper.ToDomain(updatedRequestDto);
    return updatedRequestDto;
    
}




// Método para registrar a atualização
private async Task LogUpdateOperation(string staffEmail, OperationRequestDto updatedRequestDto)
{
    var log = new Log(
        new LogId(Guid.NewGuid().ToString()),
        new ActionType(ActionTypeEnum.Update),
        new Email(staffEmail),
        new Text($"Operation request {updatedRequestDto.RequestId} updated by doctor {staffEmail} at {DateTime.UtcNow}.")
    );

    var logDataModel = LogMapper.ToDataModel(log);
    await _context.Logs.AddAsync(logDataModel);
    await _context.SaveChangesAsync();
}

public async Task<OperationRequestDto> DeleteAsync(OperationRequestDto updatedRequestDto)
{
    var existingRequest = await _context.OperationRequests
        .FirstOrDefaultAsync(r => r.RequestId == updatedRequestDto.RequestId);

    if (existingRequest == null)
    {
        throw new Exception("Operation request not found.");
    }

    var doctor =  await _context.Staff
        .FirstOrDefaultAsync(s => s.StaffId == updatedRequestDto.StaffId);
    
    var doctorEmail = await _context.Users
        .Where(u => u.Id == doctor.Email)
        .Select(u => u.Id)
        .FirstOrDefaultAsync();

    if (existingRequest.StaffId != doctorEmail)
    {
        throw new Exception("Only the requesting doctor can delete this operation request.");
    }

    await LogDeleteOperation(doctorEmail, updatedRequestDto);
    _context.OperationRequests.Remove(existingRequest);
    await _context.SaveChangesAsync();

    return updatedRequestDto;
}

private async Task LogDeleteOperation(string doctorEmail, OperationRequestDto updatedRequestDto){
    var log = new Log(
        new LogId(Guid.NewGuid().ToString()),
        new ActionType(ActionTypeEnum.Delete),
        new Email(doctorEmail),
        new Text($"Operation request {updatedRequestDto.RequestId} deleted by doctor {doctorEmail} at {DateTime.UtcNow}.")
    );

    var logDataModel = LogMapper.ToDataModel(log);
    await _context.Logs.AddAsync(logDataModel);
    await _context.SaveChangesAsync();
}
    
    }
}
