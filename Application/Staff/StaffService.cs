
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using BackOffice.Application.Logs;
using BackOffice.Application.Services;
using BackOffice.Application.Staffs;
using BackOffice.Domain.Logs;
using BackOffice.Domain.Shared;
using BackOffice.Domain.Staff;
using BackOffice.Domain.Users;
using BackOffice.Domain.Utilities;
using BackOffice.Infrastructure;
using BackOffice.Infrastructure.Patients;
using BackOffice.Infrastructure.Persistence.Models;
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
    private readonly IConfiguration _configuration;

    

    private readonly IEmailService _emailService;

    public StaffService(IStaffRepository staffRepository, BackOfficeDbContext dbContext, IUserRepository userRepository, IEmailService emailService,IConfiguration configuration)
    {
        _staffRepository = staffRepository;
        _dbContext = dbContext;
        _userRepository = userRepository;
        _emailService = emailService;
        _configuration = configuration;
    }

    public async Task<StaffDataModel> CreateStaffAsync(StaffDto staffDto, IConfiguration configuration)
        {
            var existingStaff = await _staffRepository.GetByLicenseNumberAsync(staffDto.LicenseNumber);
            if (existingStaff != null)
            {
                throw new Exception("A staff member with this license number already exists.");
            }

            var existingUser = await _userRepository.GetByPhoneNumberAsync(staffDto.PhoneNumber);
            if (existingUser == null)
            {
                throw new Exception("No user in the database matches this phone number.");
            }

            if (existingUser.Role == "Patient")
            {
                throw new Exception("User is not part of the Staff.");
            }

            var recruitmentYear = DateTime.Now.Year;
            StaffId staffId;
            do
            {
                var sequentialNumber = GenerateSequentialNumber();
                staffId = new StaffId(existingUser.Role, recruitmentYear, sequentialNumber);
            }
            while (await _staffRepository.GetByStaffIdAsync(staffId.AsString()) != null);

            var staffDomain = StaffMapper.ToDomain(staffDto, staffId, configuration);
            var generatedEmail = staffDomain.Email.Value;

            var existingUserDataModel = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == existingUser.Id.AsString());

            if (existingUserDataModel == null)
            {
                throw new Exception("Existing user not found in the database.");
            }
            _dbContext.Users.Remove(existingUserDataModel);
            await _dbContext.SaveChangesAsync();
            var newUserDataModel = new UserDataModel
            {
                Id = generatedEmail,
                Role = existingUser.Role,
                PhoneNumber = existingUser.PhoneNumber.Number,
                FirstName = existingUser.FirstName.NameValue,
                LastName = existingUser.LastName.NameValue,
                FullName = existingUser.FullName.NameValue,
                Active = existingUser.Active,
                IsToBeDeleted = existingUser.IsToBeDeleted,
                ActivationToken = existingUser.ActivationToken,
                TokenExpiration = existingUser.TokenExpiration
            };

            _dbContext.Users.Add(newUserDataModel);
            await _dbContext.SaveChangesAsync();

            var staffDataModel = StaffMapper.ToDataModel(staffDomain);
            _dbContext.Staff.Add(staffDataModel);
            await _dbContext.SaveChangesAsync();

            return staffDataModel;
        }

    private int GenerateSequentialNumber()
        {
            var random = new Random();
            return random.Next(0, 99999);
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
            

        public async Task<StaffDto> UpdateAsync(StaffDto staffDto)
        {
            var staffDataModel = await _dbContext.Staff
                .Include(s => s.AvailableSlots)
                .FirstOrDefaultAsync(s => s.StaffId == staffDto.StaffId);

            if (staffDataModel == null)
            {
                throw new Exception("Staff member not found.");
            }
            foreach (var slot in staffDto.AvailableSlots)
            {
                Console.WriteLine($"SlotDto Id: {slot.Id}, StartTime: {slot.StartTime}, EndTime: {slot.EndTime}");
            }
            var user = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Id == staffDataModel.StaffId.ToLower() + "@myhospital.com");

            if (user == null)
            {
                throw new Exception("User associated with this staff not found.");
            }

            bool phoneNumberChanged = false;
            bool specChanged = false;
            bool isUpdated = false;

            isUpdated |= UpdateProperties(staffDto, staffDataModel, ref phoneNumberChanged, ref specChanged);
            isUpdated |= UpdateProperties(staffDto, user, ref phoneNumberChanged, ref specChanged);

            var updatedAvailableSlotDataModels = staffDto.AvailableSlots
                .Select(slotDto => StaffMapper.ToDataModel1(slotDto))
                .ToList();

            if (updatedAvailableSlotDataModels != null && updatedAvailableSlotDataModels.Count > 0)
            {
                foreach (var updatedSlotDataModel in updatedAvailableSlotDataModels)
                {
                    var existingSlot = staffDataModel.AvailableSlots
                        .FirstOrDefault(s => s.Id == updatedSlotDataModel.Id);

                    if (existingSlot != null)
                    {
                        if (existingSlot.StartTime != updatedSlotDataModel.StartTime || existingSlot.EndTime != updatedSlotDataModel.EndTime)
                        {
                            Console.WriteLine($"Updating existing slot with ID: {existingSlot.Id}");
                            existingSlot.StartTime = updatedSlotDataModel.StartTime;
                            existingSlot.EndTime = updatedSlotDataModel.EndTime;
                            isUpdated = true;
                            _dbContext.Entry(existingSlot).State = EntityState.Modified;  
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Adding new slot for StaffId: {staffDto.StaffId}");
                        var newSlot = new AvailableSlotDataModel
                        {
                            StartTime = updatedSlotDataModel.StartTime,
                            EndTime = updatedSlotDataModel.EndTime,
                            StaffId = staffDto.StaffId
                        };

                        staffDataModel.AvailableSlots.Add(newSlot);
                        _dbContext.Entry(newSlot).State = EntityState.Added;
                        isUpdated = true;
                    }
                }
            }

            if (!isUpdated)
            {
                var updatedStaffDomain = StaffMapper.ToDomain(staffDataModel, new StaffId(staffDto.StaffId), _configuration);
                return StaffMapper.ToDto(updatedStaffDomain);
            }
            _dbContext.Staff.Update(staffDataModel);
            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();

            staffDataModel = await _dbContext.Staff
                .Include(s => s.AvailableSlots)
                .FirstOrDefaultAsync(s => s.StaffId == staffDto.StaffId);

            if (phoneNumberChanged && _emailService != null)
            {
                await _emailService.SendEmailAsync(user.Id, "Phone number change", $"Your phone number has been changed to {staffDto.PhoneNumber}.");
            }
            var updatedStaffDomainFinal = StaffMapper.ToDomain(staffDataModel, new StaffId(staffDto.StaffId), _configuration);
            await LogUpdateOperation(user.Id);
            return StaffMapper.ToDto(updatedStaffDomainFinal);
        }
        private bool UpdateProperties(object source, object target, ref bool phoneNumberChanged, ref bool specChanged)
        {
            bool isUpdated = false;

            foreach (var property in source.GetType().GetProperties())
            {
                if (typeof(System.Collections.IEnumerable).IsAssignableFrom(property.PropertyType) && property.PropertyType != typeof(string))
                {
                    continue;
                }

                var sourceValue = property.GetValue(source);
                var targetProperty = target.GetType().GetProperty(property.Name);
                if (targetProperty != null)
                {
                    var targetValue = targetProperty.GetValue(target);

                    if (sourceValue != null && !sourceValue.Equals(targetValue))
                    {
                        targetProperty.SetValue(target, sourceValue);
                        isUpdated = true;

                        if (property.Name == "PhoneNumber")
                        {
                            phoneNumberChanged = true;
                        }

                        if (property.Name == "Specialization")
                        {
                            specChanged = true;
                        }
                    }
                }
            }

            return isUpdated;
        }

        private async Task LogDeactivateOperation(string userEmail, StaffDataModel staffDataModel)
        {
            var log = new Log(
                new LogId(Guid.NewGuid().ToString()),
                new ActionType(ActionTypeEnum.Delete),
                new Email(userEmail),
                new Text($"Staff Profile {userEmail} deactivated by admin at {DateTime.UtcNow}.")
            );

            var logDataModel = LogMapper.ToDataModel(log);
            await _dbContext.Logs.AddAsync(logDataModel);
            await _dbContext.SaveChangesAsync();
        }

        private async Task LogUpdateOperation(string userEmail)
        {
            var log = new Log(
                new LogId(Guid.NewGuid().ToString()),
                new ActionType(ActionTypeEnum.Update),
                new Email(userEmail),
                new Text($"Staff Profile {userEmail} updated by admin at {DateTime.UtcNow}.")
            );

            var logDataModel = LogMapper.ToDataModel(log);
            await _dbContext.Logs.AddAsync(logDataModel);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<StaffDto> DeactivateStaff(LicenseNumber licenseNumber)
        {
            var staff = await _staffRepository.GetByLicenseNumberAsync(licenseNumber.AsString());
            if (staff == null)
            {
                throw new Exception("Staff member not found.");
            }

            var user = await _userRepository.GetByEmailAsync(staff.Email);
            if (user == null)
            {
                throw new Exception("User not found.");
            }

            var staff1 = StaffMapper.ToDto(staff);
            staff1.Status = false;
            await _staffRepository.UpdateAsync(staff);
            await _dbContext.SaveChangesAsync();
            await LogDeactivateOperation(user.Id.AsString(), staff);

            var staffDto = StaffMapper.ToDto(staff);
            return staffDto;
        }




    }
}
