using BackOffice.Domain.OperationRequest;
using BackOffice.Domain.Patients;
using BackOffice.Domain.Staff;
using BackOffice.Infraestructure.NeededPersonnel;
using System.ComponentModel.DataAnnotations;

namespace BackOffice.Infraestructure.Appointement
{
    public class AppointementDataModel
    {
        [Key]
        public required Guid AppointementId { get; set; } // Primary key
        public required DateTime Schedule { get; set; }
        public string? Request { get; set; }
        public required string Patient { get; set; }
        public required string Staff { get; set; }
        public required List<NeededPersonnelDataModel> AllocatedStaff{ get; set; } = new List<NeededPersonnelDataModel>();
        public required List<SurgeryPhaseDataModel> SurgeryPhases{ get; set; }  = new List<SurgeryPhaseDataModel>();


    }
}
