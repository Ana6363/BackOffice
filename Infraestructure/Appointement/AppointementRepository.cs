using BackOffice.Domain.Appointement;
using BackOffice.Domain.OperationRequest;
using BackOffice.Infraestructure.OperationRequest;
using BackOffice.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace BackOffice.Infraestructure.Appointement
{
    public class AppointementRepository : IAppointementRepository
    {
        private readonly BackOfficeDbContext _context;

        public AppointementRepository(BackOfficeDbContext context)
        {
            _context = context;
        }

      /*   public async Task<AppointementDataModel> AddAsync(Domain.Appointement.Appointement appointement)  {
            if (appointement == null)
            {
                throw new ArgumentNullException(nameof(appointement), "Appointement cannot be null.");
            }

            // Map the appointement to AppointementDataModel
            var appointementDataModel = new AppointementDataModel
            {
                AppointementId = Guid.Parse(appointement.Id.AsString()),
                Schedule = appointement.Schedule.Value,
                Request = appointement.Request.AsString(),
                Patient = appointement.Patient.AsString(),
                Staff = appointement.Staff.AsString()
            };

            // Add the appointement to the database
            await _context.Appointements.AddAsync(appointementDataModel);
            await _context.SaveChangesAsync(); // Save first to get the AppointementId

            // Add related NeededPersonnel (AllocatedStaff)
            if (appointement.AllocatedStaff != null && appointement.NeededPersonnel.Any())
            {
                var neededPersonnel = appointement.NeededPersonnel.Select(personnel => new NeededPersonnelDataModel
                {
                    StaffId = personnel.StaffId,
                    Specialization = personnel.Specialization,
                    Appointement = appointementDataModel // Link to saved appointement
                }).ToList();

                await _context.NeededPersonnel.AddRangeAsync(neededPersonnel);
                await _context.SaveChangesAsync();
            }

            // Add related SurgeryPhases
            if (appointement.SurgeryPhases != null && appointement.SurgeryPhases.Any())
            {
                var surgeryPhases = appointement.SurgeryPhases.Select(phase => new SurgeryPhasesDataModel
                {
                    RoomNumber = phase.RoomNumber,
                    PhaseType = phase.PhaseType,
                    Duration = phase.Duration,
                    StartTime = phase.StartTime,
                    EndTime = phase.EndTime,
                    AppointementId = appointementDataModel.AppointementId.ToString() // Link to saved appointement
                }).ToList();

                await _context.SurgeryPhases.AddRangeAsync(surgeryPhases);
                await _context.SaveChangesAsync();
            }

            return appointementDataModel;
        }
        */

        public async Task<AppointementDataModel?> GetByIdAsync(AppointementId id)
        {
            var appointementIdString = id.AsString();

            return await _context.Appointements
                .FirstOrDefaultAsync(p => p.AppointementId.ToString() == appointementIdString);
        }

        public async Task<List<AppointementDataModel>> GetAllAsync()
        {
            return await _context.Appointements.ToListAsync();
        }

        public async Task UpdateAsync(Domain.Appointement.Appointement appointement)
        {
            var appointementDataModel = await GetByIdAsync(appointement.Id);
            if (appointementDataModel == null)
            {
                throw new Exception("Appointement not found."); // Handle not found case
            }

            appointementDataModel.Patient = appointement.Patient.AsString();
            appointementDataModel.Request = appointement.Request.AsString();
            appointementDataModel.Schedule = appointement.Schedule.Value;
            appointementDataModel.Staff = appointement.Staff.AsString();

            _context.Appointements.Update(appointementDataModel);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(AppointementId id)
        {
            var appointementDataModel = await GetByIdAsync(id);
            if (appointementDataModel != null)
            {
                _context.Appointements.Remove(appointementDataModel);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<AppointementDataModel?> GetByRequestIdAsync(RequestId requestId)
        {
            var appointement = await _context.Appointements
                .FirstOrDefaultAsync(p => p.Request.ToString() == requestId.AsString());

            return appointement;
        }
    }
}
