using BackOffice.Domain.OperationType;
using BackOffice.Application.OperationTypeService;
using BackOffice.Domain.Staff;

namespace BackOffice.Application.OperationTypeService
{
    public static class OperationTypeMapper
    {   
        public static OperationTypeDTO ToOperationTypeDTO(OperationType operationType)
        {
            return new OperationTypeDTO()
            {
                OperationTypeId = operationType.ToString(),
                OperationTime = operationType.OperationTime.AsFloat(),
                OperationTypeName = operationType.OperationTypeName.ToString(),
                Specializations = operationType.Specializations?.Select(s => s.ToString()).ToList()
            };

        }


        public static OperationType ToDomain(OperationTypeDTO operationTypeDTO){
            return new OperationType(
                operationTypeDTO.OperationTypeId,
                new OperationTypeName(operationTypeDTO.OperationTypeName),
                new OperationTime(operationTypeDTO.OperationTime),
                operationTypeDTO.Specializations?.Select(s => Specializations.FromString(s)).ToList() ?? new List<Specializations>()
            );
        }
    }
}