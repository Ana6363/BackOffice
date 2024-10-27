using BackOffice.Domain.OperationType;
using BackOffice.Application.OperationTypes;
using BackOffice.Domain.Staff;
using BackOffice.Infrastructure.OperationTypes;

namespace BackOffice.Application.OperationTypes
{
    public static class OperationTypeMapper
    {   
        // domain to dto
        public static OperationTypeDTO ToOperationTypeDTO(OperationType operationType)
        {
            return new OperationTypeDTO()
            {
                OperationTypeId = operationType.Id.AsString(),
                OperationTime = operationType.OperationTime.AsFloat(),
                OperationTypeName = operationType.OperationTypeName.ToString(),
                //Specializations = operationType.Specializations?.Select(s => s.ToString()).ToList()
                Specializations = operationType.Specializations?.Select(s => new SpecializationDTO 
                {
                    Name = s.ToString()
                }).ToList() ?? new List<SpecializationDTO>()
                };

        }

        // convert from dto to domain
        public static OperationType ToDomain(OperationTypeDTO operationTypeDTO){
            return new OperationType(
                operationTypeDTO.OperationTypeId,
                new OperationTypeName(operationTypeDTO.OperationTypeName),
                new OperationTime(operationTypeDTO.OperationTime),
                operationTypeDTO.Specializations?.Select(s => Specializations.FromString(s.Name)).ToList() ?? new List<Specializations>()

            );
            
        }

        // domain to data model
        /* public static OperationTypeDataModel toDataModel(OperationType operationType)
        {
            if(operationType == null)
            {

            }
            return new OperationTypeDataModel
            {
                OperationTypeId = operationType.ToString(),
                OperationTime = operationType.OperationTime.AsFloat(),
                OperationTypeName = operationType.OperationTypeName.ToString(),


            };
        } */


        public static OperationType fromDataModelToDomain(OperationTypeDataModel operationTypeDataModel)
        {
            var domainSpecializations = operationTypeDataModel.Specializations
                .Select(s => Specializations.FromString(s.Name)) 
                .ToList();

            return new OperationType(
                operationTypeDataModel.OperationTypeId,
                new OperationTypeName(operationTypeDataModel.OperationTypeName),
                new OperationTime(operationTypeDataModel.OperationTime),
                domainSpecializations
            );
        }
    }
}