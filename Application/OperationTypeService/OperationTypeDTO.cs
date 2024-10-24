using BackOffice.Domain.Staff;

namespace BackOffice.Application.OperationTypeService
{
    public class OperationTypeDTO
    {
        public string OperationTypeId { get; set; } 
        public string OperationTypeName { get; set; } 
        public float OperationTime { get; set; } 
        public List<string> Specializations { get; set; }
    }

}