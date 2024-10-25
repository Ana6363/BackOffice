using BackOffice.Domain.OperationRequest;
using BackOffice.Domain.OperationType;
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
                request.StaffId.AsString(),
                request.Status.Value.ToString(),
                request.OperationTypeId.Name
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

            if(!Enum.TryParse<Status.StatusType>(requestDto.Status, true, out var statusType))
            {
                throw new ArgumentException("Invalid status", nameof(requestDto.Status));
            }

            return new BackOffice.Domain.OperationRequest.OperationRequest(
                new RequestId((Guid)requestDto.RequestId!),
                new DeadLine(requestDto.DeadLine),
                new Priority(priorityType), 
                new RecordNumber(requestDto.RecordNumber),
                new StaffId(requestDto.StaffId),
                new Status(statusType),
                new OperationTypeName(requestDto.OperationTypeName)
            );
        }

        public static OperationRequestDataModel ToDataModel(Domain.OperationRequest.OperationRequest request) 
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request), "Request cannot be null.");

            return new OperationRequestDataModel
            {
                RequestId = Guid.Parse(request.RequestId.AsString()),
                DeadLine = request.DeadLine.Value,
                Priority = request.Priority.Value.ToString(),
                RecordNumber = request.Patient.AsString(),
                StaffId = request.StaffId.AsString(),
                Status = request.Status.Value.ToString(),
                OperationType = request.OperationTypeId.Name
            };
        }

        public static BackOffice.Domain.OperationRequest.OperationRequest toDomain(OperationRequestDataModel request){
            if (request == null)
                throw new ArgumentNullException(nameof(request), "Request cannot be null.");

            if (!Enum.TryParse<Priority.PriorityType>(request.Priority, true, out var priorityType))
            {
                throw new ArgumentException("Invalid priority", nameof(request.Priority));
            }

            if(!Enum.TryParse<Status.StatusType>(request.Status, true, out var statusType))
            {
                throw new ArgumentException("Invalid status", nameof(request.Status));
            }

            return new BackOffice.Domain.OperationRequest.OperationRequest(
                new RequestId((Guid)request.RequestId),
                new DeadLine(request.DeadLine),
                new Priority(priorityType), 
                new RecordNumber(request.RecordNumber),
                new StaffId(request.StaffId),
                new Status(statusType),
                new OperationTypeName(request.OperationType)
            );
        }

    }
}
