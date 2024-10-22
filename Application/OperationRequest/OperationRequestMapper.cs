using BackOffice.Domain.OperationRequest;
using BackOffice.Domain.Patients;
using BackOffice.Domain.Staff;
using BackOffice.Domain.Users;
using BackOffice.Infraestructure.OperationRequest;
using BackOffice.Infrastructure.Patients;

namespace BackOffice.Application.OperationRequest
{
    public class OperationRequestMapper
    {
        public static OperationRequestDto ToDto(Domain.OperationRequest.OperationRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request), "Request cannot be null.");

            return new OperationRequestDto(
                Guid.Parse(request.Id.AsString()), 
                request.DeadLine.Value,
                request.Priority.Value.ToString(),
                request.Patient.AsString(),
                request.StaffId.AsString()
            );
        }

        public static BackOffice.Domain.OperationRequest.OperationRequest ToDomain(OperationRequestDto requestDto)
        {
            if (requestDto == null)
                throw new ArgumentNullException(nameof(requestDto), "RequestDto cannot be null.");

            if (!Enum.TryParse<Priority.PriorityType>(requestDto.Priority, true, out var priorityType))
            {
                throw new ArgumentException("Invalid priority", nameof(requestDto.Priority));
            }

            return new BackOffice.Domain.OperationRequest.OperationRequest(
                new RequestId((Guid)requestDto.RequestId!),
                new DeadLine(requestDto.DeadLine),
                new Priority(priorityType), // Fix for Problem 3
                new RecordNumber(requestDto.RecordNumber),
                new LicenseNumber(requestDto.StaffId)
            );
        }

        public static OperationRequestDataModel ToDataModel(Domain.OperationRequest.OperationRequest request) // Fix for Problem 1
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request), "Request cannot be null.");

            return new OperationRequestDataModel
            {
                RequestId = Guid.Parse(request.RequestId.AsString()),
                DeadLine = request.DeadLine.Value,
                Priority = request.Priority.Value.ToString(),
                RecordNumber = request.Patient.AsString(),
                StaffId = request.StaffId.AsString()
            };
        }

    }
}
