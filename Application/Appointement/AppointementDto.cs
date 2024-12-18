using BackOffice.Domain.Staff;
using BackOffice.Domain.SurgeryPhase;

namespace BackOffice.Application.Appointement
{
    public class AppointementDto
    {
        public Guid? AppointementId { get; set; }
        public DateTime Schedule { get; set; }
        public Guid? Request { get; set; }
        public string Patient { get; set; }
        public string Staff { get; set; }
        public string? RoomNumber { get; set; }
        public List<NeededPersonnelDto> NeededPersonnel { get; set; } = new List<NeededPersonnelDto>();
        public List<SurgeryPhaseDto>? SurgeryPhases { get; set; } = new List<SurgeryPhaseDto>();

        // Parameterless constructor required for deserialization
        public AppointementDto() { }
    }
}
