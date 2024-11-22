namespace BackOffice.Application.OperationRequest
{
    public class FilteredRequestDto
    {
        public string? PatientName { get; set; } 
        public string? Priority { get; set; }
        public string? Status { get; set; }
        public string? StaffId { get; set; }
        public string? TypeId { get; set; }


        public FilteredRequestDto(string? patientName, string? priority, string? status,string? staffId, string? typeId)
        {
            PatientName = patientName;
            Priority = priority;
            Status = status;
            StaffId = staffId;
            TypeId = typeId;
        }

        public FilteredRequestDto() { }
    }
}
