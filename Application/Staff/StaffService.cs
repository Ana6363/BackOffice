
using System.Threading.Tasks;
using BackOffice.Application.Staffs;
using BackOffice.Domain.Staff;
using BackOffice.Domain.Users;
using BackOffice.Infrastructure;
using BackOffice.Infrastructure.Staff;
using Microsoft.Extensions.Configuration;


namespace BackOffice.Application.StaffService
{
    public class StaffService
    {
        private readonly IStaffRepository _staffRepository;
        private readonly BackOfficeDbContext _dbContext;

        private readonly IUserRepository _userRepository;
        public StaffService(IStaffRepository staffRepository, BackOfficeDbContext dbContext, IUserRepository userRepository)
        {
            _staffRepository = staffRepository;
            _dbContext = dbContext;
            _userRepository = userRepository;
        }

        public async Task<StaffDataModel> CreateStaffAsync(StaffDto staffDto, IConfiguration configuration)
            {
                var existingStaff = await _staffRepository.GetByLicenseNumberAsync(staffDto.LicenseNumber);
                if (existingStaff != null)
                {
                    throw new Exception("A staff member with this license number already exists.");
                }
                var existingUser = await _userRepository.GetByEmailAsync(staffDto.Email);
                if (existingUser == null)
                {
                    throw new Exception("No user in the database matches this email.");
                }

                if (existingUser.Role == "Patient")
                {
                    throw new Exception("User is not part of the Staff.");
                }

                // Pass the configuration to the ToDomain method
                var staffDomain = StaffMapper.ToDomain(staffDto, configuration);

                var staffDataModel = StaffMapper.ToDataModel(staffDomain);
                _dbContext.Staff.Add(staffDataModel);
                await _dbContext.SaveChangesAsync();

                return staffDataModel;
            }


    }
}
