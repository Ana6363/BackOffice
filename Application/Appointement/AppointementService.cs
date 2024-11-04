using BackOffice.Application.Logs;
using BackOffice.Application.OperationRequest;
using BackOffice.Domain.Appointement;
using BackOffice.Domain.Logs;
using BackOffice.Domain.OperationRequest;
using BackOffice.Domain.Shared;
using BackOffice.Domain.Users;
using BackOffice.Infraestructure.Appointement;
using BackOffice.Infrastructure;
using System.Security.Claims;

namespace BackOffice.Application.Appointement
{
    public class AppointementService
    {
        private readonly IAppointementRepository _appointementRepository;
        private readonly BackOfficeDbContext _context;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AppointementService(IAppointementRepository appointementRepository, IUnitOfWork unitOfWork, BackOfficeDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _appointementRepository = appointementRepository;
            _unitOfWork = unitOfWork;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<AppointementDataModel> CreateAppointementAsync(AppointementDto appointement)
        {
            if (appointement == null)
            {
                Console.WriteLine("Error mapping");
                throw new Exception("Appointement is null");
            }

            var appointementDto = new AppointementDto(
                Guid.NewGuid(),
                appointement.Schedule,
                appointement.Request,
                appointement.Patient,
                appointement.Staff
            );

            var staff = _context.Staff
                .FirstOrDefault(s => s.StaffId == appointementDto.Staff);
            var slots = staff.AvailableSlots;

            foreach (var slot in slots)
            {
                if (appointement.Schedule != slot.StartTime)
                {
                    Console.WriteLine("Slot Unavailable");
                    throw new Exception("Slot Unavailable");
                }
            }

            Console.WriteLine($"Date in DTO: {appointementDto.Schedule}");

            var appointement1 = AppointementMapper.ToDomain(appointementDto);
            Console.WriteLine($"Date in Domain: {appointement.Schedule.ToString()}");

            if (appointement1 == null)
            {
                Console.WriteLine("Error mapping");
                throw new Exception("Appointement is null");
            }

            try
            {
                return await _appointementRepository.AddAsync(appointement1);
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

        public async Task<AppointementDto> UpdateAppointementAsync(AppointementDto updatedAppointement)
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

        }

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

