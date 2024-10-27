using BackOffice.Domain.OperationType;
using BackOffice.Application.OperationTypes;
using BackOffice.Domain.Staff;
using BackOffice.Infrastructure.OperationTypes;

namespace BackOffice.Application.OperationTypes
{
    public static class OperationTypeMapper
{
    public static OperationTypeDTO ToOperationTypeDTO(OperationType operationType)
    {
        return new OperationTypeDTO()
        {
            OperationTypeId = operationType.Id.AsString(),
            OperationTime = operationType.OperationTime.AsFloat(),
            OperationTypeName = operationType.OperationTypeName.Name,
            Specializations = operationType.Specializations?.Select(s => new SpecializationDTO 
            {
                Name = s.Value.ToString()
            }).ToList() ?? new List<SpecializationDTO>()
        };
    }

    public static OperationType ToDomain(OperationTypeDTO operationTypeDTO, OperationTypeId id)
    {
        return new OperationType(
            id,
            new OperationTypeName(operationTypeDTO.OperationTypeName),
            new OperationTime(operationTypeDTO.OperationTime),
            operationTypeDTO.Specializations?.Select(s => Specializations.FromString(s.Name)).ToList() ?? new List<Specializations>()
        );
    }

    public static OperationTypeDataModel ToDataModel(OperationType operationType)
    {
        return new OperationTypeDataModel
        {
            OperationTypeId = operationType.Id.AsString(),
            OperationTime = operationType.OperationTime.AsFloat(),
            OperationTypeName = operationType.OperationTypeName.Name,
        };
    }

    public static OperationType FromDataModelToDomain(OperationTypeDataModel dataModel)
{
    return new OperationType(
        new OperationTypeId(dataModel.OperationTypeId),
        new OperationTypeName(dataModel.OperationTypeName),
        new OperationTime(dataModel.OperationTime),
        dataModel.Specializations.Select(s => Specializations.FromString(s.Name)).ToList()
    );
}


}
}
