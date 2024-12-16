using BackOffice.Application.Logs;
using BackOffice.Application.OperationRequest;
using BackOffice.Domain.Appointement;
using BackOffice.Domain.Logs;
using BackOffice.Domain.OperationRequest;
using BackOffice.Domain.Shared;
using BackOffice.Domain.SurgeryPhase;
using BackOffice.Domain.Users;
using BackOffice.Infraestructure.Appointement;
using BackOffice.Infrastructure;
using Healthcare.Domain.Services;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BackOffice.Application.Appointement
{
    public class AppointementService
    {
        private readonly IAppointementRepository _appointementRepository;
        private readonly BackOfficeDbContext _context;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly SurgeryRoomService _surgeryRoomService;
        private readonly OperationRequestService _operationRequestService;

        public AppointementService(IAppointementRepository appointementRepository, IUnitOfWork unitOfWork, BackOfficeDbContext context, IHttpContextAccessor httpContextAccessor, SurgeryRoomService surgeryRoomService, OperationRequestService operationRequestService)
        {
            _appointementRepository = appointementRepository;
            _unitOfWork = unitOfWork;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _surgeryRoomService = surgeryRoomService;
            _operationRequestService = operationRequestService;
        }

       public async Task<AppointementDataModel> CreateAppointementAsync(AppointementDto appointementDto)
        {
            if (appointementDto == null)
            {
                Console.WriteLine("Error mapping");
                throw new Exception("Appointement is null");
            }

            Guid appointementId = Guid.NewGuid();
            Console.WriteLine(appointementDto.Schedule);

            try
            {
                var operationRequest = await _context.OperationRequests
                    .FirstOrDefaultAsync(or => or.RequestId == appointementDto.Request);

                if (operationRequest == null)
                    throw new Exception("Operation Request not found");

                string operationTypeName = operationRequest.OperationType;

                var operationType = await _context.OperationType
                    .FirstOrDefaultAsync(ot => ot.OperationTypeName == operationTypeName);

                if (operationType == null)
                    throw new Exception("Operation Type not found");

                int preparationTime = operationType.PreparationTime;
                int surgeryTime = operationType.SurgeryTime;
                int cleaningTime = operationType.CleaningTime;

                DateTime preparationDate = appointementDto.Schedule;
                DateTime surgeryDate = preparationDate.AddMinutes(preparationTime);
                DateTime cleaningDate = surgeryDate.AddMinutes(surgeryTime);
                DateTime endTime = cleaningDate.AddMinutes(cleaningTime);
                string roomNumber = await _surgeryRoomService.GetAvailableRoomAsync(preparationDate, endTime);

                var preparationPhase = new SurgeryPhaseDto
                {
                    RoomNumber = roomNumber,
                    PhaseType = "Preparation",
                    Duration = preparationTime,
                    StartTime = preparationDate,
                    EndTime = surgeryDate,
                    AppointementId = appointementId.ToString()
                };

                var surgeryPhase = new SurgeryPhaseDto
                {
                    RoomNumber = roomNumber,
                    PhaseType = "Surgery",
                    Duration = surgeryTime,
                    StartTime = surgeryDate,
                    EndTime = cleaningDate,
                    AppointementId = appointementId.ToString()
                };

                var cleaningPhase = new SurgeryPhaseDto
                {
                    RoomNumber = roomNumber,
                    PhaseType = "Cleaning",
                    Duration = cleaningTime,
                    StartTime = cleaningDate,
                    EndTime = cleaningDate.AddMinutes(cleaningTime),
                    AppointementId = appointementId.ToString()
                };

                appointementDto.SurgeryPhases = new List<SurgeryPhaseDto> { preparationPhase, surgeryPhase, cleaningPhase };

                AppointementDataModel appointementDataModel = AppointementMapper.ToDataModel(appointementDto);
                var opRequestDto = new OperationRequestDto(
                    appointementDto.Request,
                    operationRequest.DeadLine,
                    "MEDIUM",
                    operationRequest.RecordNumber,
                    operationRequest.StaffId,
                    "ACCEPTED",
                    operationRequest.OperationType
                );

                await _operationRequestService.UpdateAsync(opRequestDto);

                _context.Appointements.Add(appointementDataModel);
                await _context.SaveChangesAsync();

                if (appointementDto.SurgeryPhases != null && appointementDto.SurgeryPhases.Any())
                {
                    foreach (var phase in appointementDto.SurgeryPhases)
                    {
                        var surgeryPhaseDataModel = new SurgeryPhaseDataModel
                        {
                            RoomNumber = phase.RoomNumber,
                            PhaseType = phase.PhaseType,
                            Duration = phase.Duration ?? 0,
                            StartTime = phase.StartTime ?? throw new Exception("StartTime cannot be null"),
                            EndTime = phase.EndTime ?? throw new Exception("EndTime cannot be null"),
                            AppointementId = new Guid(appointementDataModel.AppointementId.ToString())
                        };
                        _context.SurgeryPhaseDataModel.Add(surgeryPhaseDataModel);
                    }
                    await _context.SaveChangesAsync();
                }


                Console.WriteLine("Appointement and phases saved successfully.");

                return appointementDataModel;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error adding to repo: {e.Message}");
                throw;
            }
        }


        public async Task<IEnumerable<AppointementDataModel>> GetAppointementsAsync ()
        {
            var appointements = await _appointementRepository.GetAllAsync();
            return appointements;
        }

        public async Task<AppointementDataModel> GetAppointementByIdAsync(AppointementId id)
        {
            var appointement = await _appointementRepository.GetByIdAsync(id);
            return appointement;
        }

        public async Task<AppointementDataModel> GetAppointementByRequestIdAsync(RequestId requestId)
        {
            var appointement = await _appointementRepository.GetByRequestIdAsync(requestId);
            return appointement;
        }

        
    /*    public async Task<AppointementDto> UpdateAppointementAsync(AppointementDto updatedAppointement)
        {
            var existingAppointement = _context.Appointements
                .FirstOrDefault(a => a.AppointementId == updatedAppointement.AppointementId);
            if (existingAppointement == null)
            {
                Console.WriteLine("Error updating");
                throw new Exception("Appointement not found");
            }

            var staff = _context.Staff
                .FirstOrDefault(s => s.StaffId == updatedAppointement.Staff);
            if (staff == null)
            {
                Console.WriteLine("Error updating");
                throw new Exception("Staff not found");
            }

            var getLoggedUser = GetLoggedInUserEmail();
            var loggedUserId = getLoggedUser.Split("@")[0];
            if(!loggedUserId.Equals(existingAppointement.Staff, StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception("Only the requesting doctor can update this operation request.");
            }

            bool isUpdated = false;
            if (updatedAppointement.Schedule != existingAppointement.Schedule)
            {
                existingAppointement.Schedule = updatedAppointement.Schedule;
                isUpdated = true;
            }

            if(isUpdated)
            {
                await LogUpdateAppointement(loggedUserId, updatedAppointement);
                await _context.SaveChangesAsync();
            }

            var updatedAppointement1 = AppointementMapper.ToDomain(updatedAppointement);
            return updatedAppointement;

        } */

        private string GetLoggedInUserEmail()
        {
            var claimsIdentity = _httpContextAccessor.HttpContext.User.Identity as ClaimsIdentity;
            if (claimsIdentity != null)
            {
                var emailClaim = claimsIdentity.FindFirst(ClaimTypes.Email);
                if (emailClaim != null)
                {
                    Console.WriteLine(emailClaim.Value);
                    return emailClaim.Value;
                }
            }

            throw new Exception("User email not found in token.");
        }

        private async Task LogUpdateAppointement(string staffEmail, AppointementDto updatedRequestDto)
        {
            var log = new Log(
                new LogId(Guid.NewGuid().ToString()),
                new ActionType(ActionTypeEnum.Update),
                new Email(staffEmail),
                new Text($"Operation request {updatedRequestDto.AppointementId} updated by doctor {staffEmail} at {DateTime.UtcNow}.")
            );

            var logDataModel = LogMapper.ToDataModel(log);
            await _context.Logs.AddAsync(logDataModel);
            await _context.SaveChangesAsync();
        }

        public async Task<AppointementDataModel> DeleteAppointementAsync(AppointementId id)
        {
            var appointement = await _appointementRepository.GetByIdAsync(id);
            if (appointement == null)
            {
                Console.WriteLine("Error deleting");
                throw new Exception("Appointement not found");
            }

            var getLoggedUser = GetLoggedInUserEmail();
            var loggedUserId = getLoggedUser.Split("@")[0];
            if (!loggedUserId.Equals(appointement.Staff, StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception("Only the requesting doctor can delete this operation request.");
            }


            await LogDeleteOperation(loggedUserId, appointement);
            _context.Appointements.Remove(appointement);
            await _appointementRepository.DeleteAsync(id);
            await _context.SaveChangesAsync();
            await _unitOfWork.CommitAsync();
            return appointement;
        }

        private async Task LogDeleteOperation(string doctorEmail, AppointementDataModel updatedRequestDto)
        {
            var log = new Log(
                new LogId(Guid.NewGuid().ToString()),
                new ActionType(ActionTypeEnum.Delete),
                new Email(doctorEmail),
                new Text($"Operation request {updatedRequestDto.AppointementId} deleted by doctor {doctorEmail} at {DateTime.UtcNow}.")
            );

            var logDataModel = LogMapper.ToDataModel(log);
            await _context.Logs.AddAsync(logDataModel);
            await _context.SaveChangesAsync();
        }

    }
}

