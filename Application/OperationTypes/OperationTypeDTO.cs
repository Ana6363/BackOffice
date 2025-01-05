using BackOffice.Domain.Staff;

namespace BackOffice.Application.OperationTypes
{
    public class OperationTypeDTO
    {
        public string? OperationTypeId { get; set; } 
        public string OperationTypeName { get; set; } 
        public int PreparationTime { get; set; } 
        public int SurgeryTime { get; set; } 
        public int CleaningTime { get; set; } 
        public List<OpTypeRequirementsDTO> Specializations { get; set; }
    }

}