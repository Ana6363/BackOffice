namespace BackOffice.Application.OperationRequest
{
    public class FilteredRequestDto
    {
        public string? PatientName { get; set; } 
        public string? Priority { get; set; }
        public string? Status { get; set; }


        public FilteredRequestDto(string? patientName, string? priority, string? status)
        {
            PatientName = patientName;
            Priority = priority;
            Status = status;
        }

        public FilteredRequestDto() { }
    }
}
