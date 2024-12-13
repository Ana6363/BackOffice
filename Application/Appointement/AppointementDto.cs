using BackOffice.Domain.Staff;

namespace BackOffice.Application.Appointement
{
    public class AppointementDto
    {
        public Guid? AppointementId { get; set; }
        public DateTime Schedule { get; set; }
        public Guid? Request { get; set; }
        public string Patient { get; set; }
        public string Staff { get; set; }

        public List<NeededPersonnelDto> NeededPersonnel { get; set; } = new List<NeededPersonnelDto();

        public AppointementDto(Guid? appointementId, DateTime schedule, Guid request, string patient, string staff, List<NeededPersonnelDto> neededPersonnel)
        {
            AppointementId = appointementId;
            Schedule = schedule;
            Request = request;
            Patient = patient;
            Staff = staff;
            NeededPersonnel = neededPersonnel;
        }


    }
}
