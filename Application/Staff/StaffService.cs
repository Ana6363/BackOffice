
using System.Threading.Tasks;
using BackOffice.Application.Staffs;
using BackOffice.Domain.Staff;
using BackOffice.Domain.Users;
using BackOffice.Infrastructure;
using BackOffice.Infrastructure.Staff;
using Microsoft.EntityFrameworkCore;
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

        public async Task<IEnumerable<StaffDataModel>> GetFilteredStaffAsync(StaffFilterDto filterDto)
            {
                var query = _dbContext.Staff
                    .Include(s => s.AvailableSlots) 
                    .Join(_dbContext.Users, staff => staff.Email, user => user.Id, (staff, user) => new { staff, user });

                if (filterDto.PhoneNumber.HasValue)
                {
                    query = query.Where(s => EF.Functions.Collate(s.user.PhoneNumber.ToString(), "utf8mb4_unicode_ci") == EF.Functions.Collate(filterDto.PhoneNumber.Value.ToString(), "utf8mb4_unicode_ci"));
                }
                if (!string.IsNullOrWhiteSpace(filterDto.FirstName))
                {
                    query = query.Where(s => EF.Functions.Collate(s.user.FirstName, "utf8mb4_unicode_ci").Contains(EF.Functions.Collate(filterDto.FirstName, "utf8mb4_unicode_ci")));
                }
                if (!string.IsNullOrWhiteSpace(filterDto.LastName))
                {
                    query = query.Where(s => EF.Functions.Collate(s.user.LastName, "utf8mb4_unicode_ci").Contains(EF.Functions.Collate(filterDto.LastName, "utf8mb4_unicode_ci")));
                }
                if (!string.IsNullOrWhiteSpace(filterDto.FullName))
                {
                    query = query.Where(s => EF.Functions.Collate(s.user.FullName, "utf8mb4_unicode_ci").Contains(EF.Functions.Collate(filterDto.FullName, "utf8mb4_unicode_ci")));
                }

                var result = await query
                    .Select(s => s.staff)
                    .ToListAsync();

                return result;
            }




    }
}
