using BackOffice.Domain.OperationType;
using BackOffice.Application.OperationTypes;
using BackOffice.Infrastructure.OperationTypes;
using BackOffice.Domain.Specialization;

namespace BackOffice.Application.OperationTypes
{
    public static class OperationTypeMapper
{
    public static OperationTypeDTO ToOperationTypeDTO(OperationType operationType)
    {
        return new OperationTypeDTO()
        {
            OperationTypeId = operationType.Id.AsString(),
            PreparationTime = operationType.PreparationTime.time,
            SurgeryTime = operationType.PreparationTime.time,
            CleaningTime = operationType.PreparationTime.time,
            OperationTypeName = operationType.OperationTypeName.Name,
            Specializations = operationType.Specializations?.Select(s => new OpTypeRequirementsDTO 
            {
                Name = s.Value.ToString(),
            }).ToList() ?? new List<OpTypeRequirementsDTO>()
        };
    }

 /*  public static OperationType ToDomain(OperationTypeDTO operationTypeDTO, OperationTypeId id)
    {
        return new OperationType(
            id,
            new OperationTypeName(operationTypeDTO.OperationTypeName),
            new OperationTime(operationTypeDTO.PreparationTime),
            new OperationTime(operationTypeDTO.SurgeryTime),
            new OperationTime(operationTypeDTO.CleaningTime)
            operationTypeDTO.Specializations?.Select(s => Specializations.AsString(s.Name)).ToList() ?? new List<Specializations>()
        );
    } */

    public static OperationTypeDataModel ToDataModel(OperationType operationType)
    {
        return new OperationTypeDataModel
        {
            OperationTypeId = operationType.Id.AsString(),
            PreparationTime = operationType.PreparationTime.time,
            SurgeryTime = operationType.SurgeryTime.time,
            CleaningTime = operationType.CleaningTime.time,
            OperationTypeName = operationType.OperationTypeName.Name,
        };
    }

    public static OperationType FromDataModelToDomain(OperationTypeDataModel dataModel)
{
    return new OperationType(
        new OperationTypeId(dataModel.OperationTypeId),
        new OperationTypeName(dataModel.OperationTypeName),
        new OperationTime(dataModel.PreparationTime),
        new OperationTime(dataModel.SurgeryTime),
        new OperationTime(dataModel.CleaningTime),
        dataModel.Specializations
            .Select(s => new Specializations(s.Name)) // Convert to Specializations
            .ToList()
    );
}

public static OperationTypeDataModel ToDataModel(OperationTypeDTO dto)
{
    return new OperationTypeDataModel
    {
        OperationTypeId = dto.OperationTypeId,
        PreparationTime = dto.PreparationTime,
        SurgeryTime = dto.SurgeryTime,
        CleaningTime = dto.CleaningTime,
        OperationTypeName = dto.OperationTypeName,
        Specializations = dto.Specializations?.Select(s => new OpTypeRequirementsDataModel
        {
            SpecializationId = Guid.NewGuid().ToString(), // Generate a unique ID for each specialization
            Name = s.Name,
            NeededPersonnel = s.NeededPersonnel ?? 0, // Provide default if NeededPersonnel is null
            OperationTypeId = dto.OperationTypeId // Set the foreign key
        }).ToList() ?? new List<OpTypeRequirementsDataModel>()
    };
}




}
}
