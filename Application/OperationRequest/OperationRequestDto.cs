namespace BackOffice.Application.OperationRequest
{
    public class OperationRequestDto
    {
        public Guid? RequestId { get; set; }
        public DateTime DeadLine { get; set; }
        public string Priority { get; set; }
        public string RecordNumber { get; set; }
        public string StaffId { get; set; }
        public string Status { get; set; }

        public OperationRequestDto(Guid? requestId, DateTime deadLine, string priority, string recordNumber, string staffId, string status) 
        {
            RequestId = requestId;
            DeadLine = deadLine;
            Priority = priority;
            RecordNumber = recordNumber;
            StaffId = staffId;
            Status = status;
        }
    }
}
