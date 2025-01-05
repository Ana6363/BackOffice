using BackOffice.Domain.Appointement;
using BackOffice.Domain.OperationRequest;
using BackOffice.Domain.Patients;
using BackOffice.Domain.Staff;
using BackOffice.Infraestructure.Appointement;
using BackOffice.Infraestructure.NeededPersonnel;

namespace BackOffice.Application.Appointement
{
    public class AppointementMapper
    {

        public static AppointementDataModel ToDataModel(AppointementDto appointementDto)
{
    if (appointementDto == null)
        throw new ArgumentNullException(nameof(appointementDto), "Appointement cannot be null.");

    return new AppointementDataModel
    {
        AppointementId = appointementDto.AppointementId ?? Guid.NewGuid(), // Generate a new ID if null
        Request = appointementDto.Request?.ToString(), // Convert nullable Guid to string
        Schedule = appointementDto.Schedule,
        Patient = appointementDto.Patient ?? throw new ArgumentNullException(nameof(appointementDto.Patient)),
        Staff = appointementDto.Staff ?? throw new ArgumentNullException(nameof(appointementDto.Staff)),

        AllocatedStaff = appointementDto.NeededPersonnel?
            .Select(np => new NeededPersonnelDataModel
            {
                StaffId = np.StaffId,
                Specialization = np.Specialization
            }).ToList() ?? new List<NeededPersonnelDataModel>(),

        SurgeryPhases = new List<SurgeryPhaseDataModel>()
    };
}

    }
}
