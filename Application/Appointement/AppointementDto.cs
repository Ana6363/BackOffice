namespace BackOffice.Application.Appointement
{
    public class AppointementDto
    {
        public Guid? AppointementId { get; set; }
        public DateTime Schedule { get; set; }
        public string? Request { get; set; }
        public string Patient { get; set; }
        public string Staff { get; set; }

        public AppointementDto(Guid? appointementId, DateTime schedule, string request, string patient, string staff)
        {
            AppointementId = appointementId;
            Schedule = schedule;
            Request = request;
            Patient = patient;
            Staff = staff;
        }


    }
}
