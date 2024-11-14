using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BackOffice.Application.Patients;
using BackOffice.Domain.Patients;
using BackOffice.Application.Users;
using BackOffice.Domain.Users;
using BackOffice.Infrastructure.Patients;
using BackOffice.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using BackOffice.Infrastructure.Services;
using BackOffice.Domain.Shared;
using BackOffice.Application.Services;
using System.Reflection;
using BackOffice.Domain.Logs;
using BackOffice.Application.Logs;
using System.Security.Claims;

namespace BackOffice.Application.Patients
{
    public class PatientService
    {
        private readonly IPatientRepository _patientRepository;
        private readonly UserService _userService;
        private readonly BackOfficeDbContext _dbContext;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public PatientService(IPatientRepository patientRepository, UserService userService, BackOfficeDbContext dbContext, IUnitOfWork unitOfWork, IEmailService emailService ,IHttpContextAccessor httpContextAccessor)
        {
            _patientRepository = patientRepository;
            _userService = userService;
            _dbContext = dbContext;
            _unitOfWork = unitOfWork;
            _emailService = emailService;
            _httpContextAccessor = httpContextAccessor;
        }

        private string GetLoggedInUserRole()
        {
            var claimsIdentity = _httpContextAccessor.HttpContext.User.Identity as ClaimsIdentity;
            if (claimsIdentity != null)
            {
                var roleClaim = claimsIdentity.FindFirst(ClaimTypes.Role);
                if (roleClaim != null)
                {
                    Console.WriteLine(roleClaim.Value);
                    return roleClaim.Value;
                }
            }

            throw new Exception("User email not found in token.");
        }

        private string GetLoggedInUserEmail()
        {
            var claimsIdentity = _httpContextAccessor.HttpContext.User.Identity as ClaimsIdentity;
            if (claimsIdentity != null)
            {
                var emailClaim = claimsIdentity.FindFirst(ClaimTypes.Email);
                if (emailClaim != null)
                {
                    Console.WriteLine(emailClaim.Value);
                    return emailClaim.Value;
                }
            }

            throw new Exception("User email not found in token.");
        }


        public async Task<PatientDataModel> CreatePatientAsync(PatientDto patientDto)
        {
            var existingUser = await _userService.GetByIdAsync(new UserId(patientDto.UserId));
            if (existingUser != null)
            {
                throw new Exception("User is already registered in the database.");
            }

            var existingUserWithPhoneNumber = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.PhoneNumber == patientDto.PhoneNumber);

            if (existingUserWithPhoneNumber != null)
            {
                throw new Exception("A user with this phone number already exists. Cannot create a new patient with this phone number.");
            }


            var userDto = new UserDto
            {
                Id = patientDto.UserId,
                Role = "Patient",
                Active = false,
                PhoneNumber = patientDto.PhoneNumber,
                ActivationToken = null,
                TokenExpiration = null,
                FirstName = patientDto.FirstName,
                LastName = patientDto.LastName,
                FullName = patientDto.FullName
            };

            await _userService.AddAsync(userDto);

            var latestPatient = await _dbContext.Patients
                .OrderByDescending(p => p.RecordNumber)
                .FirstOrDefaultAsync();

            int nextRecordNumber = latestPatient != null
                ? int.Parse(latestPatient.RecordNumber) + 1
                : 1;

            string generatedRecordNumber = nextRecordNumber.ToString("D5");

            patientDto.RecordNumber = generatedRecordNumber;

            var patient = PatientMapper.ToDomain(patientDto);

            if (patient == null)
            {
                Console.WriteLine("Mapped patient object is null.");
                throw new Exception("Error mapping the patient object.");
            }

            try
            {
                return await _patientRepository.AddAsync(patient);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding patient: {ex.Message}");
                throw;
            }
        }

        public async Task<PatientDto> UpdateAsync(PatientDto patientDto)
        {
            var currentUserRole = GetLoggedInUserRole();
            var currentUserEmail = GetLoggedInUserEmail();

            var patientDataModel = await _dbContext.Patients
                .FirstOrDefaultAsync(p => p.RecordNumber == patientDto.RecordNumber);

            if (patientDataModel == null)
            {
                throw new Exception("Patient not found.");
            }

            // Check if the user is an Admin or the owner of the record
            if (currentUserRole != "Admin" && currentUserEmail != patientDataModel.UserId)
            {
                throw new UnauthorizedAccessException("You are not authorized to update this patient's information.");
            }

            // Proceed with the update logic if authorized
            var user = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Id == patientDataModel.UserId);

            if (user == null)
            {
                throw new Exception("User associated with this patient not found.");
            }

            // Check if the phone number change is allowed
            if (patientDto.PhoneNumber != user.PhoneNumber)
            {
                var existingUserWithSamePhoneNumber = await _dbContext.Users
                    .FirstOrDefaultAsync(u => u.PhoneNumber == patientDto.PhoneNumber);

                if (existingUserWithSamePhoneNumber != null)
                {
                    throw new Exception("A user with this phone number already exists. Cannot update to this phone number.");
                }

                user.PhoneNumber = patientDto.PhoneNumber;
            }

            bool phoneNumberChanged = false;
            bool emergencyContactChanged = false;
            bool isUpdated = false;
            
            isUpdated |= UpdateProperties(patientDto, patientDataModel, ref phoneNumberChanged, ref emergencyContactChanged);
            isUpdated |= UpdateProperties(patientDto, user, ref phoneNumberChanged, ref emergencyContactChanged);

            if (!isUpdated)
            {
                var updatedPatientDomain = PatientMapper.ToDomain(patientDataModel);
                return PatientMapper.ToDto(updatedPatientDomain);
            }

            _dbContext.Patients.Update(patientDataModel);
            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();

            if (phoneNumberChanged)
            {
                await _emailService.SendEmailAsync(user.Id, "Phone number change", $"Your phone number has been changed to {patientDto.PhoneNumber}.");
            }

            if (emergencyContactChanged)
            {
                await _emailService.SendEmailAsync(user.Id, "Emergency contact change", $"Your emergency contact has been changed to {patientDto.EmergencyContact}.");
            }

            var updatedPatientDomainFinal = PatientMapper.ToDomain(patientDataModel);
            await LogUpdateOperation(patientDto.UserId, patientDto);
            return PatientMapper.ToDto(updatedPatientDomainFinal);
        }


        // Helper method to update properties and detect changes
        private bool UpdateProperties(object source, object target, ref bool phoneNumberChanged, ref bool emergencyContactChanged)
        {
            bool isUpdated = false;

            foreach (PropertyInfo property in source.GetType().GetProperties())
            {
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

                        if (property.Name == "EmergencyContact")
                        {
                            emergencyContactChanged = true;
                        }
                    }
                }
            }

            return isUpdated;
        }


        public async Task MarkToDelete(RecordNumber recordNumber)
        {
            var patientDataModel = await _patientRepository.GetByIdAsync(recordNumber);
            if (patientDataModel == null)
            {
                throw new Exception("Patient not found.");
            }

            var userDataModel = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Id == patientDataModel.UserId);

            if (userDataModel == null)
            {
                throw new Exception("User associated with this patient not found.");
            }

            Console.WriteLine("HHHHHHH");
            userDataModel.IsToBeDeleted = true;

            Console.WriteLine("HHHHHHH");
            _dbContext.Users.Update(userDataModel);

            Console.WriteLine("HHHHHHH");
            await _dbContext.SaveChangesAsync();
        }

        public async Task<PatientDataModel?> DeletePatientAsync(RecordNumber recordNumber)
        {
            var currentUserRole = GetLoggedInUserRole();
            var currentUserEmail = GetLoggedInUserEmail();

            var patient = await _patientRepository.GetByIdAsync(recordNumber);
            if (patient == null)
            {
                throw new Exception("Patient not found.");
            }

            // Check if the user is an Admin or the owner of the record
            if (currentUserRole != "Admin" && currentUserEmail != patient.UserId)
            {
                throw new UnauthorizedAccessException("You are not authorized to delete this patient's information.");
            }

            var user = await _userService.GetByIdAsync(new UserId(patient.UserId));
            if (user == null || patient.UserId != user.Id)
            {
                throw new Exception("Patient does not belong to the user.");
            }

            // Skip 'IsToBeDeleted' check if the user is deleting their own account
            if (currentUserRole == "Admin" || currentUserEmail != user.Id)
            {
                if (!user.IsToBeDeleted)
                {
                    throw new Exception("User is not marked for deletion.");
                }

                // Notify user if they are an admin
                await _emailService.SendEmailAsync(user.Id, "Account Deletion Scheduled", 
                    $"Your account and related patient information will be deleted in 24 hours.");

                // Delay deletion for admins
                _ = Task.Delay(TimeSpan.FromHours(24)).ContinueWith(async _ =>
                {
                    try
                    {
                        await _patientRepository.DeleteAsync(recordNumber);
                        await _userService.DeleteAsync(new UserId(patient.UserId));
                        await _unitOfWork.CommitAsync();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error during scheduled deletion: {ex.Message}");
                    }
                });
            }
            else
            {
                 _ = Task.Delay(TimeSpan.FromHours(24)).ContinueWith(async _ =>{
                await _patientRepository.DeleteAsync(recordNumber);
                await _userService.DeleteAsync(new UserId(patient.UserId));
                await _unitOfWork.CommitAsync();
                });
            }

            // Log the deletion operation
            await LogDeleteOperation(user.Id, patient);
            return patient;
        }



        private async Task LogDeleteOperation(string userEmail, PatientDataModel patientDataModel)
        {
            var log = new Log(
                new LogId(Guid.NewGuid().ToString()),
                new ActionType(ActionTypeEnum.Delete),
                new Email(userEmail),
                new Text($"Patient profile {userEmail} scheduled for deleting in 24 hours by an admin at {DateTime.UtcNow}.")
            );

            var logDataModel = LogMapper.ToDataModel(log);
            await _dbContext.Logs.AddAsync(logDataModel);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<PatientUserInfoDto>> GetFilteredPatientsAsync(PatientFilterDto filterDto)
    {
        var query = from patient in _dbContext.Patients
                    join user in _dbContext.Users on patient.UserId equals user.Id
                    select new { patient, user };

        if (!string.IsNullOrWhiteSpace(filterDto.UserId))
        {
            query = query.Where(p => p.user.Id == filterDto.UserId);
        }
        if (filterDto.PhoneNumber != null)
        {
            query = query.Where(u => u.user.PhoneNumber == filterDto.PhoneNumber);
        }
        if (!string.IsNullOrWhiteSpace(filterDto.FirstName))
        {
            query = query.Where(p => p.user.FirstName.Contains(filterDto.FirstName));
        }
        if (!string.IsNullOrWhiteSpace(filterDto.LastName))
        {
            query = query.Where(p => p.user.LastName.Contains(filterDto.LastName));
        }
        if (!string.IsNullOrWhiteSpace(filterDto.FullName))
        {
            query = query.Where(p => p.user.FullName.Contains(filterDto.FullName));
        }
        if (filterDto.IsToBeDeleted.HasValue)
        {
            query = query.Where(s => s.user.IsToBeDeleted == filterDto.IsToBeDeleted.Value);
        }

        var result = await query
            .Select(p => new PatientUserInfoDto
            {
                Patient = p.patient,
                PhoneNumber = p.user.PhoneNumber,
                UserId = p.user.Id,
                IsToBeDeleted = p.user.IsToBeDeleted
            })
            .ToListAsync();

        return result;
    }


        private async Task LogUpdateOperation(string userEmail, PatientDto patientDto)
        {
            var log = new Log(
                new LogId(Guid.NewGuid().ToString()),
                new ActionType(ActionTypeEnum.Update),
                new Email(userEmail),
                new Text($"Patient profile {userEmail} updated by an admin at {DateTime.UtcNow}.")
            );

            var logDataModel = LogMapper.ToDataModel(log);
            await _dbContext.Logs.AddAsync(logDataModel);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<PatientUserInfoDto>> GetLoggedPatientAsync()
        {
            var loggedInUserEmail = GetLoggedInUserEmail();

            var query = from patient in _dbContext.Patients
                        join user in _dbContext.Users on patient.UserId equals user.Id
                        where user.Id == loggedInUserEmail
                        select new { patient, user };

            var result = await query
                .Select(p => new PatientUserInfoDto
                {
                    Patient = p.patient,
                    PhoneNumber = p.user.PhoneNumber,
                    UserId = p.user.Id,
                    IsToBeDeleted = p.user.IsToBeDeleted,
                    FirstName = p.user.FirstName,
                    LastName = p.user.LastName,
                    FullName = p.user.FullName
                })
                .ToListAsync();

            return result;
        }


}

    
}