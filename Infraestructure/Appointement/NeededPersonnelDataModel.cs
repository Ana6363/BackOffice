using BackOffice.Domain.OperationRequest;
using BackOffice.Domain.Patients;
using BackOffice.Domain.Staff;
using BackOffice.Infraestructure.Appointement;
using System.ComponentModel.DataAnnotations;

namespace BackOffice.Infraestructure.NeededPersonnel
{
    public class NeededPersonnelDataModel
    {     
        public required string StaffId { get; set; }

        public required string Specialization { get; set; }

        public Guid AppointementId { get; set; }

        public AppointementDataModel Appointement { get; set; }
        

    }
}
