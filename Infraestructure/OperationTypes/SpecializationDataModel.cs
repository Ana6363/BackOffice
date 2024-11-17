using System.ComponentModel.DataAnnotations;

namespace BackOffice.Infrastructure.OperationTypes
{
    public class SpecializationDataModel
    {
        [Key]
        public required string SpecializationId { get; set; }
        public required string Name { get; set; } 
        public int NeededPersonnel { get; set; } 
        public required string OperationTypeId { get; set; } 
        public OperationTypeDataModel OperationType { get; set; } 
    }
}
