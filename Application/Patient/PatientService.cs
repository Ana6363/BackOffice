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

namespace BackOffice.Application.Patients
{
    public class PatientService
    {
        private readonly IPatientRepository _patientRepository;
        private readonly UserService _userService;
        private readonly BackOfficeDbContext _dbContext;
        private readonly UnitOfWork _unitOfWork;
        private readonly EmailService _emailService;

        public PatientService(IPatientRepository patientRepository, UserService userService, BackOfficeDbContext dbContext, UnitOfWork unitOfWork, EmailService emailService)
        {
            _patientRepository = patientRepository;
            _userService = userService;
            _dbContext = dbContext;
            _unitOfWork = unitOfWork;
            _emailService = emailService;
        }

        public async Task<PatientDataModel> CreatePatientAsync(PatientDto patientDto)
        {
            var existingUser = await _userService.GetByIdAsync(new UserId(patientDto.UserId));
            if (existingUser != null)
            {
                throw new Exception("User is already registered in the database.");
            }

            var existingPatientWithPhoneNumber = await _dbContext.Patients
                .FirstOrDefaultAsync(p => p.PhoneNumber == patientDto.PhoneNumber);

            if (existingPatientWithPhoneNumber != null)
            {
                throw new Exception("A patient with this phone number already exists.");
            }

            var userDto = new UserDto
            {
                Id = patientDto.UserId,
                Role = "Patient",
                Active = false,
                ActivationToken = null,
                TokenExpiration = null,
                FirstName = patientDto.FirstName,
                LastName = patientDto.LastName,
                FullName = patientDto.FullName
            };

            await _userService.AddAsync(userDto);

            // Generate the next sequential RecordNumber
            var latestPatient = await _dbContext.Patients
                .OrderByDescending(p => p.RecordNumber)
                .FirstOrDefaultAsync();

            int nextRecordNumber = latestPatient != null
                ? int.Parse(latestPatient.RecordNumber) + 1
                : 1;

            string generatedRecordNumber = nextRecordNumber.ToString("D5"); // Always 5 digits

            // Ensure the generated RecordNumber is set
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
        public async Task<PatientDto> UpdateAsync(RecordNumber recordNumber, PatientDto patientDto)
        {
            var patient = await _patientRepository.GetByIdAsync(recordNumber);
            if (patient == null)
            {
                throw new Exception("Patient not found.");
            }
            if (patient.UserId != patientDto.UserId)
            {
                throw new Exception("Patient does not belong to the user.");
            }
            var user = await _userService.GetByIdAsync(new UserId(patient.UserId));
            if (patient.EmergencyContact != patientDto.EmergencyContact)
            {
                if (_emailService != null)
                {
                    await _emailService.SendEmailAsync(user.Id, "Emergency contact change", $"Your emergency contact has been changed to {patientDto.PhoneNumber}.");
                }
                patient.EmergencyContact = patientDto.EmergencyContact;
            }
            if (patient.PhoneNumber != patientDto.PhoneNumber)
            {
                if (_emailService != null)
                {
                    await _emailService.SendEmailAsync(user.Id, "Phone number change", $"Your phone number has been changed to {patientDto.PhoneNumber}.");
                }
                patient.PhoneNumber = patientDto.PhoneNumber;
            }

            await _patientRepository.UpdateAsync(PatientMapper.ToDomain(patient));
            await _unitOfWork.CommitAsync();

            return PatientMapper.ToDto(PatientMapper.ToDomain(patient));
        }

        public async Task<PatientDto?> MarkToDelete(RecordNumber recordNumber)
        {
            var patient = await _patientRepository.GetByIdAsync(recordNumber);
            if (patient == null)
            {
                throw new Exception("Patient not found.");
            }

            patient.IsToBeDeleted = true;
            await _patientRepository.UpdateAsync(PatientMapper.ToDomain(patient)); // Fix: Pass the correct type
            await _unitOfWork.CommitAsync();

            return PatientMapper.ToDto(PatientMapper.ToDomain(patient)); // Fix: Pass the correct type
        }


        public async Task<PatientDataModel?> DeletePatientAsync(RecordNumber recordNumber)
        {
            var patient = await _patientRepository.GetByIdAsync(recordNumber);
            if (patient == null)
            {
                throw new Exception("Patient not found.");
            }
            var user = await _userService.GetByIdAsync(new UserId(patient.UserId));
            if (patient.UserId != user.Id)
            {
                throw new Exception("Patient does not belong to the user.");
            }
            if (user?.Active == true)
            {
                throw new Exception("Cannot delete a patient from an active user.");
            }
            if (!patient.IsToBeDeleted)
            {
                throw new Exception("Patient isn't marked for deletion.");
            }
            await _patientRepository.DeleteAsync(recordNumber);
            await _userService.DeleteAsync(new UserId(patient.UserId));
            await _unitOfWork.CommitAsync();

            return patient;
        }

        public async Task<IEnumerable<PatientDataModel>> GetFilteredPatientsAsync(PatientFilterDto filterDto)
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
                query = query.Where(p => p.patient.PhoneNumber == filterDto.PhoneNumber);
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

            var result = await query.Select(p => p.patient).ToListAsync();
            return result;
        }
    }
}