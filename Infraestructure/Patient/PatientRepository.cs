using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BackOffice.Domain.Patients;
using BackOffice.Domain.Shared;
using BackOffice.Infrastructure.Patients;
using Microsoft.EntityFrameworkCore;

namespace BackOffice.Infrastructure.Persistence.Repositories
{
    public class PatientRepository : IPatientRepository
    {
        private readonly BackOfficeDbContext _context;

        public PatientRepository(BackOfficeDbContext context)
        {
            _context = context;
        }
        public async Task<PatientDataModel> AddAsync(Patient patient)
        {
            if (patient == null)
            {
                throw new ArgumentNullException(nameof(patient), "Patient cannot be null.");
            }
            var patientDataModel = new PatientDataModel
            {
                RecordNumber = patient.RecordNumber.AsString(),
                UserId = patient.UserId.AsString(),
                DateOfBirth = patient.DateOfBirth.Value,
                PhoneNumber = patient.PhoneNumber.Number,
                EmergencyContact = patient.EmergencyContact.Number,
                Gender = patient.Gender.ToString() ?? "UNKNOWN",
            };

            await _context.Patients.AddAsync(patientDataModel);
            await _context.SaveChangesAsync();

            return patientDataModel;
        }
        public async Task<PatientDataModel?> GetByIdAsync(RecordNumber id)
        {
            var recordNumberString = id.AsString();

            return await _context.Patients
                .FirstOrDefaultAsync(p => p.RecordNumber == recordNumberString);
        }

        public async Task<List<PatientDataModel>> GetAllAsync()
        {
            return await _context.Patients.ToListAsync();
        }

        public async Task UpdateAsync(Patient patient)
        {
            var patientDataModel = await GetByIdAsync(patient.Id);
            if (patientDataModel == null)
            {
                throw new Exception("Patient not found."); // Handle not found case
            }

            patientDataModel.DateOfBirth = patient.DateOfBirth.Value;
            patientDataModel.PhoneNumber = patient.PhoneNumber.Number; 
            patientDataModel.EmergencyContact = patient.EmergencyContact.Number; 
            patientDataModel.Gender = patient.Gender.Value.ToString();

            _context.Patients.Update(patientDataModel);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(RecordNumber id)
        {
            var patientDataModel = await GetByIdAsync(id);
            if (patientDataModel != null)
            {
                _context.Patients.Remove(patientDataModel);
                await _context.SaveChangesAsync();
            }
        }
    }
}
