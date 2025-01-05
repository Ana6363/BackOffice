using BackOffice.Domain.Shared;
using BackOffice.Domain.Specialization;

namespace BackOffice.Domain.OperationType
{
    public class OperationType : Entity<OperationTypeId>, IAggregateRoot
    {
        public OperationTypeName OperationTypeName { get; private set; }
        public OperationTime PreparationTime { get; private set; }
        public OperationTime SurgeryTime { get; private set; }
        public OperationTime CleaningTime { get; private set; }
        public List<Specializations> Specializations { get; private set; }
        
        public OperationType()
        {
            Specializations = new List<Specializations>();
        }

        public OperationType(OperationTypeId id, OperationTypeName operationTypeName, OperationTime preparationTime,OperationTime surgeryTime,OperationTime cleaningTime, List<Specializations> specializations)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            OperationTypeName = operationTypeName ?? throw new ArgumentNullException(nameof(operationTypeName));
            PreparationTime = preparationTime;
            SurgeryTime = surgeryTime;
            CleaningTime = cleaningTime;
            Specializations = specializations ?? new List<Specializations>();
        }
    } 
}
