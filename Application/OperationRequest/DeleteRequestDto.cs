namespace BackOffice.Application.OperationRequest
{
    public class DeleteRequestDto
    {
        public Guid? RequestId { get; set; }

        public DeleteRequestDto(Guid? requestId)
        {
            RequestId = requestId;

        }
      
    }
}
