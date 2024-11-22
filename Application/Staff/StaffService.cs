
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
    var staffId = staffDto.StaffId.Split('@')[0]; // Extract part before '@'

    var staffEmail = staffDto.StaffId; // Use the provided email directly

    // Check if the user with the staff email already exists in the database
    var existingUser = await _userRepository.GetByEmailAsync(staffEmail);
    if (existingUser == null)
    {
        throw new Exception("No user in the database matches this staff ID email.");
    }

    if (existingUser.Role == "Patient")
    {
        throw new Exception("User is not part of the Staff.");
    }

    // Check if a staff member with this license number already exists
    var existingStaff = await _staffRepository.GetByLicenseNumberAsync(staffDto.LicenseNumber);
    if (existingStaff != null)
    {
        throw new Exception("A staff member with this license number already exists.");
    }

    // Map to the domain model
    var staffIdDomain = new StaffId(staffId);
    var staffDomain = StaffMapper.ToDomain(staffDto, staffIdDomain, configuration);

    // Create and save the staff data model without deleting the user
    var staffDataModel = StaffMapper.ToDataModel(staffDomain);
    staffDataModel.Email = staffEmail;  // Associate with the user's email

    _dbContext.Staff.Add(staffDataModel);
    await _dbContext.SaveChangesAsync();

    // Associate generated staffId with available slots
    if (staffDto.AvailableSlots != null && staffDto.AvailableSlots.Any())
    {
        foreach (var slot in staffDto.AvailableSlots)
        {
            var slotDataModel = new AvailableSlotDataModel
            {
                StaffId = staffId, // Associate the slot with the generated staffId
                StartTime = slot.StartTime,
                EndTime = slot.EndTime
            };

            _dbContext.AvailableSlots.Add(slotDataModel);
        }

        await _dbContext.SaveChangesAsync();
    }

    return staffDataModel;
}



        public async Task<IEnumerable<object>> GetFilteredStaffAsync(StaffFilterDto filterDto)
        {
            var query = _dbContext.Staff
                .Include(s => s.AvailableSlots) 
                .Join(_dbContext.Users, staff => staff.Email, user => user.Id, (staff, user) => new { staff, user });

            if (!string.IsNullOrWhiteSpace(filterDto.StaffId))
            {
                query = query.Where(s => EF.Functions.Collate(s.staff.StaffId, "utf8mb4_unicode_ci") == EF.Functions.Collate(filterDto.StaffId, "utf8mb4_unicode_ci"));
            }
            if (filterDto.PhoneNumber.HasValue)
            {
                query = query.Where(s => EF.Functions.Collate(s.user.PhoneNumber.ToString(), "utf8mb4_unicode_ci") == EF.Functions.Collate(filterDto.PhoneNumber.Value.ToString(), "utf8mb4_unicode_ci"));
            }
            if (!string.IsNullOrEmpty(filterDto.Specialization))
            {
                query = query.Where(s => EF.Functions.Collate(s.staff.Specialization, "utf8mb4_unicode_ci") == EF.Functions.Collate(filterDto.Specialization, "utf8mb4_unicode_ci"));
            }
            if (filterDto.Status.HasValue)
            {
                query = query.Where(s => EF.Functions.Collate(s.staff.Status.ToString(), "utf8mb4_unicode_ci") == EF.Functions.Collate(filterDto.Status.Value.ToString(), "utf8mb4_unicode_ci"));
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
                .Select(s => new 
                {
                    StaffId = s.staff.StaffId,
                    LicenseNumber = s.staff.LicenseNumber,
                    Specialization = s.staff.Specialization,
                    Status = s.staff.Status,
                    AvailableSlots = s.staff.AvailableSlots,
                    FirstName = s.user.FirstName,
                    LastName = s.user.LastName,
                    FullName = s.user.FullName,
                    PhoneNumber = s.user.PhoneNumber
                })
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

       public async Task<StaffDto> DeactivateStaff(StaffDeactivateDto staffDeactivateDto)
{
    var staff = await _staffRepository.GetByStaffIdAsync(staffDeactivateDto.StaffId);
    if (staff == null)
    {
        throw new Exception("Staff member not found.");
    }

    var user = await _userRepository.GetByEmailAsync(staff.Email);
    if (user == null)
    {
        throw new Exception("User not found.");
    }

    staff.Status = false;
    Console.WriteLine(staff.Status);

    await _staffRepository.UpdateAsync(staff);
    await _dbContext.SaveChangesAsync();
    await LogDeactivateOperation(user.Id.AsString(), staff);

    return StaffMapper.ToDto(staff); // Ensure correct return type
}





    }
}
