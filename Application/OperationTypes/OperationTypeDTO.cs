using BackOffice.Domain.Staff;

namespace BackOffice.Application.OperationTypes
{
    public class OperationTypeDTO
    {
        public string OperationTypeId { get; set; } 
        public string OperationTypeName { get; set; } 
        public float OperationTime { get; set; } 
        public List<SpecializationDTO> Specializations { get; set; }
    }

}