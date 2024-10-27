using BackOffice.Domain.Shared;
using BackOffice.Domain.Staff;

namespace BackOffice.Domain.OperationType
{
    public class OperationType : Entity<OperationTypeId>, IAggregateRoot
    {
        public OperationTypeName OperationTypeName { get; private set; }
        public OperationTime OperationTime { get; private set; }
        public List<Specializations> Specializations { get; private set; }
        
        public OperationType()
        {
            Specializations = new List<Specializations>();
        }

        public OperationType(OperationTypeId id, OperationTypeName operationTypeName, OperationTime operationTime, List<Specializations> specializations)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            OperationTypeName = operationTypeName ?? throw new ArgumentNullException(nameof(operationTypeName));
            OperationTime = operationTime ?? throw new ArgumentNullException(nameof(operationTime));
            Specializations = specializations ?? new List<Specializations>();
        }
    } 
}
