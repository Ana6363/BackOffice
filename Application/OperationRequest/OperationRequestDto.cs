namespace BackOffice.Application.OperationRequest
{
    public class OperationRequestDto
    {
        public Guid? RequestId { get; set; }
        public DateTime DeadLine { get; set; }
        public DateTime? AppointementDate { get; set; }
        public string Priority { get; set; }
        public string RecordNumber { get; set; }
        public string StaffId { get; set; }
        public string Status { get; set; }
        public string OperationTypeName { get; set; }

        public OperationRequestDto(Guid? requestId, DateTime deadLine,DateTime? appointementDate, string priority, string recordNumber, string staffId, string status, string operationTypeName)
        {
            RequestId = requestId;
            DeadLine = deadLine;
            AppointementDate = appointementDate;
            Priority = priority;
            RecordNumber = recordNumber;
            StaffId = staffId;
            Status = status;
            OperationTypeName = operationTypeName;
        }
      
    }
}
