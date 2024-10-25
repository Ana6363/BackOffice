using System;
using BackOffice.Domain.OperationRequest;
using BackOffice.Domain.OperationType;
using BackOffice.Domain.Patients;
using BackOffice.Domain.Shared;
using BackOffice.Domain.Staff;
using Moq;
using Xunit;

namespace BackOffice.DomainTests.Test.OperationRequest
{
    public class OperationRequestTest
    {
        [Fact]
        public void CreateOperationRequest_WithValidParameters_ShouldCreateSuccessfully()
        {
            // Arrange
            var mockRequestId = new Mock<RequestId>(Guid.NewGuid());
            var mockDeadLine = new Mock<DeadLine>(DateTime.Now.AddDays(7));
            var mockPriority = new Mock<Priority>(1);
            var mockPatient = new Mock<RecordNumber>("12345");
            var mockStaffId = new Mock<StaffId>("D202412345");
            var mockStatus = new Mock<Status>("Pending");
            var mockOperationTypeId = new Mock<OperationTypeName>("Surgery");

            var mockOperationRequest = new Mock<BackOffice.Domain.OperationRequest.OperationRequest>(
                mockRequestId.Object,
                mockDeadLine.Object,
                mockPriority.Object,
                mockPatient.Object,
                mockStaffId.Object,
                mockStatus.Object,
                mockOperationTypeId.Object
            );

            // Act
            var operationRequest = mockOperationRequest.Object;

            // Assert
            Assert.Equal(mockRequestId.Object, operationRequest.RequestId);
            Assert.Equal(mockDeadLine.Object, operationRequest.DeadLine);
            Assert.Equal(mockPriority.Object, operationRequest.Priority);
            Assert.Equal(mockPatient.Object, operationRequest.Patient);
            Assert.Equal(mockStaffId.Object, operationRequest.StaffId);
            Assert.Equal(mockStatus.Object, operationRequest.Status);
            Assert.Equal(mockOperationTypeId.Object, operationRequest.OperationTypeId);
        }

        [Fact]
        public void CreateOperationRequest_WithNullRequestId_ShouldThrowException()
        {
            // Arrange
            var mockDeadLine = new Mock<DeadLine>(DateTime.Now.AddDays(7));
            var mockPriority = new Mock<Priority>(1);
            var mockPatient = new Mock<RecordNumber>("12345");
            var mockStaffId = new Mock<StaffId>("D202412345");
            var mockStatus = new Mock<Status>("Pending");
            var mockOperationTypeId = new Mock<OperationTypeName>("Surgery");

            // Act & Assert
            Assert.Throws<BusinessRuleValidationException>(() => new BackOffice.Domain.OperationRequest.OperationRequest(
                null,
                mockDeadLine.Object,
                mockPriority.Object,
                mockPatient.Object,
                mockStaffId.Object,
                mockStatus.Object,
                mockOperationTypeId.Object
            ));
        }

        [Fact]
        public void CreateOperationRequest_WithNullDeadLine_ShouldThrowException()
        {
            // Arrange
            var mockRequestId = new Mock<RequestId>(Guid.NewGuid());
            var mockPriority = new Mock<Priority>(1);
            var mockPatient = new Mock<RecordNumber>("12345");
            var mockStaffId = new Mock<StaffId>("D202412345");
            var mockStatus = new Mock<Status>("Pending");
            var mockOperationTypeId = new Mock<OperationTypeName>("Surgery");

            // Act & Assert
            Assert.Throws<BusinessRuleValidationException>(() => new BackOffice.Domain.OperationRequest.OperationRequest(
                mockRequestId.Object,
                null,
                mockPriority.Object,
                mockPatient.Object,
                mockStaffId.Object,
                mockStatus.Object,
                mockOperationTypeId.Object
            ));
        }

        [Fact]
        public void CreateOperationRequest_WithNullPatient_ShouldThrowException()
        {
            // Arrange
            var mockRequestId = new Mock<RequestId>(Guid.NewGuid());
            var mockDeadLine = new Mock<DeadLine>(DateTime.Now.AddDays(7));
            var mockPriority = new Mock<Priority>(1);
            var mockStaffId = new Mock<StaffId>("D202412345");
            var mockStatus = new Mock<Status>("Pending");
            var mockOperationTypeId = new Mock<OperationTypeName>("Surgery");

            // Act & Assert
            Assert.Throws<BusinessRuleValidationException>(() => new BackOffice.Domain.OperationRequest.OperationRequest(
                mockRequestId.Object,
                mockDeadLine.Object,
                mockPriority.Object,
                null,
                mockStaffId.Object,
                mockStatus.Object,
                mockOperationTypeId.Object
            ));
        }

        [Fact]
        public void CreateOperationRequest_WithNullStaffId_ShouldThrowException()
        {
            // Arrange
            var mockRequestId = new Mock<RequestId>(Guid.NewGuid());
            var mockDeadLine = new Mock<DeadLine>(DateTime.Now.AddDays(7));
            var mockPriority = new Mock<Priority>(1);
            var mockPatient = new Mock<RecordNumber>("12345");
            var mockStatus = new Mock<Status>("Pending");
            var mockOperationTypeId = new Mock<OperationTypeName>("Surgery");

            // Act & Assert
            Assert.Throws<BusinessRuleValidationException>(() => new BackOffice.Domain.OperationRequest.OperationRequest(
                mockRequestId.Object,
                mockDeadLine.Object,
                mockPriority.Object,
                mockPatient.Object,
                null,
                mockStatus.Object,
                mockOperationTypeId.Object
            ));
        }

        [Fact]
        public void CreateOperationRequest_WithNullOperationTypeId_ShouldThrowException()
        {
            // Arrange
            var mockRequestId = new Mock<RequestId>(Guid.NewGuid());
            var mockDeadLine = new Mock<DeadLine>(DateTime.Now.AddDays(7));
            var mockPriority = new Mock<Priority>(1);
            var mockPatient = new Mock<RecordNumber>("12345");
            var mockStaffId = new Mock<StaffId>("D202412345");
            var mockStatus = new Mock<Status>("Pending");

            // Act & Assert
            Assert.Throws<BusinessRuleValidationException>(() => new BackOffice.Domain.OperationRequest.OperationRequest(
                mockRequestId.Object,
                mockDeadLine.Object,
                mockPriority.Object,
                mockPatient.Object,
                mockStaffId.Object,
                mockStatus.Object,
                null
            ));
        }
    }
}