using System.ComponentModel.DataAnnotations;
using BackOffice.Domain.Staff;

namespace BackOffice.Infrastructure.OperationTypes
{

    public class OperationTypeDataModel
    {
        [Key]
        public required string OperationTypeId { get; set; }

        public required string OperationTypeName { get; set;}

        public required float OperationTime { get; set; }

        public List<SpecializationDataModel> Specializations {get; set;} = [];
    }
}