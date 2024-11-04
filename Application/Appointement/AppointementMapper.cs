using BackOffice.Domain.Appointement;
using BackOffice.Domain.OperationRequest;
using BackOffice.Domain.Patients;
using BackOffice.Domain.Staff;
using BackOffice.Infraestructure.Appointement;

namespace BackOffice.Application.Appointement
{
    public class AppointementMapper
    {
        public static AppointementDto ToDto(Domain.Appointement.Appointement appointement)
        {
            if (appointement == null)
                throw new ArgumentNullException(nameof(appointement), "Appointement cannot be null.");

            return new AppointementDto
            (
                Guid.Parse(appointement.Id.AsString()),
                appointement.Schedule.Value,
                appointement.Request.AsString(),
                appointement.Patient.AsString(),
                appointement.Staff.AsString()
            );
        }

        public static Domain.Appointement.Appointement ToDomain(AppointementDto appointementDto)
        {
            if (appointementDto == null)
                throw new ArgumentNullException(nameof(appointementDto), "AppointementDto cannot be null.");

            return new Domain.Appointement.Appointement(
                new AppointementId((Guid)appointementDto.AppointementId!),
                new Schedule(appointementDto.Schedule),
                new RequestId(Guid.Parse(appointementDto.Request ?? throw new ArgumentNullException(nameof(appointementDto.Request), "Request cannot be null."))),
                new RecordNumber(appointementDto.Patient),
                new StaffId(appointementDto.Staff)
            );
        }

        public static AppointementDataModel ToDataModel(Domain.Appointement.Appointement appointement)
        {
            if (appointement == null)
                throw new ArgumentNullException(nameof(appointement), "Appointement cannot be null.");

            return new AppointementDataModel
            {
                AppointementId = Guid.Parse(appointement.Id.AsString()),
                Request = appointement.Request.AsString(),
                Schedule = appointement.Schedule.Value, 
                Patient = appointement.Patient.AsString(),
                Staff = appointement.Staff.AsString() 
            };
        }
    }
}
