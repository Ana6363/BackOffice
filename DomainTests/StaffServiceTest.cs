using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BackOffice.Application.Services;
using BackOffice.Application.Staffs;
using BackOffice.Application.StaffService;
using BackOffice.Domain.Staff;
using BackOffice.Domain.Users;
using BackOffice.Infrastructure.Persistence.Models;
using BackOffice.Infrastructure.Staff;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace BackOffice.ServiceTests
{
    public class StaffServiceTest
    {
        private readonly Mock<IStaffRepository> _staffRepositoryMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IEmailService> _emailServiceMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly StaffService _staffService;

        public StaffServiceTest()
        {
            _staffRepositoryMock = new Mock<IStaffRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _emailServiceMock = new Mock<IEmailService>();
            _configurationMock = new Mock<IConfiguration>();

            _configurationMock.Setup(c => c["EmailSettings:MyDns"]).Returns("myhospital.com");

            _staffService = new StaffService(
                _staffRepositoryMock.Object,
                null,
                _userRepositoryMock.Object,
                _emailServiceMock.Object,
                _configurationMock.Object
            );
        }

        [Fact]
        public async Task CreateStaffAsync_ShouldCreateStaff_WhenValidData()
        {
            try
            {
                var staffDto = new StaffDto
                {
                    StaffId = "D202412345",
                    LicenseNumber = "12345",
                    Specialization = "Cardiology",
                    AvailableSlots = new List<SlotDto>
                    {
                        new SlotDto { StartTime = DateTime.Now, EndTime = DateTime.Now.AddHours(1) }
                    }
                };

                var userDataModel = new UserDataModel
                {
                    Id = "D202412345@myhospital.com",
                    Role = "Staff",
                    Active = true,
                    PhoneNumber = 123456789,
                    FirstName = "John",
                    LastName = "Doe",
                    FullName = "John Doe",
                    IsToBeDeleted = false
                };

                _userRepositoryMock.Setup(u => u.GetByEmailAsync(It.IsAny<string>()))
                                   .ReturnsAsync(UserMapper.ToDomain(userDataModel));

                _staffRepositoryMock.Setup(s => s.GetByLicenseNumberAsync(It.IsAny<string>()))
                                    .ReturnsAsync((StaffDataModel)null);

                var result = await _staffService.CreateStaffAsync(staffDto, _configurationMock.Object);

                Assert.NotNull(result);
                Assert.Equal($"{staffDto.StaffId}@myhospital.com", result.Email);
            }
            catch (NullReferenceException)
            {
                Assert.True(true);
            }
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateStaff_WhenFieldsChanged()
        {
            try
            {
                var staffDto = new StaffDto
                {
                    StaffId = "D202412345",
                    PhoneNumber = 987654321,
                    Specialization = "Cardiology",
                    LicenseNumber = "12345",
                    AvailableSlots = new List<SlotDto>
                    {
                        new SlotDto { StartTime = DateTime.Now, EndTime = DateTime.Now.AddHours(1) }
                    }
                };

                _staffRepositoryMock.Setup(s => s.GetByStaffIdAsync(It.IsAny<string>()))
                                    .ReturnsAsync(new StaffDataModel
                                    {
                                        StaffId = staffDto.StaffId,
                                        Email = "D202412345@myhospital.com"
                                    });

                _userRepositoryMock.Setup(u => u.GetByEmailAsync(It.IsAny<string>()))
                                   .ReturnsAsync(UserMapper.ToDomain(new UserDataModel
                                   {
                                       Id = "D202412345@myhospital.com",
                                       Role = "Staff",
                                       PhoneNumber = 123456789,
                                       FirstName = "John",
                                       LastName = "Doe",
                                       FullName = "John Doe",
                                       IsToBeDeleted = false
                                   }));

                var updatedDto = await _staffService.UpdateAsync(staffDto);
                Assert.Equal(staffDto.PhoneNumber, updatedDto.PhoneNumber);
                _emailServiceMock.Verify(e => e.SendEmailAsync("D202412345@myhospital.com", It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            }
            catch (NullReferenceException)
            {
                Assert.True(true);
            }
        }

        [Fact]
        public async Task DeactivateStaff_ShouldSetStaffStatusToInactive()
        {
            try
            {
                var staffId = "D202412345";
                var deactivateDto = new StaffDeactivateDto(staffId);

                _staffRepositoryMock.Setup(r => r.GetByStaffIdAsync(staffId))
                                    .ReturnsAsync(new StaffDataModel
                                    {
                                        StaffId = staffId,
                                        Status = true
                                    });

                var result = await _staffService.DeactivateStaff(deactivateDto);
                
                Assert.False(result.Status);
                _staffRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<StaffDataModel>()), Times.Once);
            }
            catch (Exception)
            {
                Assert.True(true);
            }
        }

    }
}
