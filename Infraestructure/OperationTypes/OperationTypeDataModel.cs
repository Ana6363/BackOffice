using System.ComponentModel.DataAnnotations;
using BackOffice.Domain.Staff;

namespace BackOffice.Infrastructure.OperationTypes
{

    public class OperationTypeDataModel
    {
        [Key]
        public required string OperationTypeId { get; set; }

        public required string OperationTypeName { get; set;}

        public required int PreparationTime { get; set; }
        public required int SurgeryTime { get; set; }
        public required int CleaningTime { get; set; }

        public List<OpTypeRequirementsDataModel> Specializations {get; set;} = [];
    }
}