    using BackOffice.Domain.Shared;
    using BackOffice.Domain.Staff;

    namespace BackOffice.Domain.OperationType;
    public class OperationType : Entity<OperationTypeId>, IAggregateRoot
    {

    public OperationTypeName OperationTypeName { get; private set; }
        public OperationTime OperationTime { get; private set; }
        
        public List<Specializations> Specializations{ get; private set; }

        public OperationType()
        {
            Specializations = new List<Specializations>();
        }
        public OperationType(string id, OperationTypeName operationTypeName, OperationTime operationTime, List<Specializations> specializations)
        {
            this.Id = OperationTypeId.NewId();
            this.OperationTypeName = operationTypeName;
            this.OperationTime = operationTime;
            this.Specializations = specializations;
        }


    } 