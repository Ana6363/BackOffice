using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BackOffice.Application.Patients;
using BackOffice.Domain.Patients;
using BackOffice.Application.Users;
using BackOffice.Domain.Users;
using BackOffice.Infrastructure.Patients;

namespace BackOffice.Application.Patients
{
    public class PatientService
    {
        private readonly IPatientRepository _patientRepository;
        private readonly UserService _userService;

        public PatientService(IPatientRepository patientRepository, UserService userService)
        {
            _patientRepository = patientRepository;
            _userService = userService;
        }

     public async Task<PatientDataModel> CreatePatientAsync(PatientDto patientDto)
{

    var existingUser = await _userService.GetByIdAsync(new UserId(patientDto.UserId));
    if (existingUser != null)
    {
        throw new Exception("User is already registered in the database.");
    }

    var userDto = new UserDto
    {
        Id = patientDto.UserId,
        Role = "Patient",
        Active = false,
        ActivationToken = null,
        TokenExpiration = null
    };

    await _userService.AddAsync(userDto);

    var patient = PatientMapper.ToDomain(patientDto);

    if (patient == null)
    {
        Console.WriteLine("Mapped patient object is null.");
    }

    try
    {
        // Saving patient
        return await _patientRepository.AddAsync(patient);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error adding patient: {ex.Message}");
        throw;
    }
}

    public async Task<IEnumerable<PatientDataModel>> GetAllPatientsAsync()
        {
            return await _patientRepository.GetAllAsync();
        }
    }
}
