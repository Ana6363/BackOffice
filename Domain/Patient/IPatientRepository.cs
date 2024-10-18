using System.Collections.Generic;
using System.Threading.Tasks;
using BackOffice.Domain.Patients;
using BackOffice.Domain.Shared;
using BackOffice.Infrastructure.Patients;
using BackOffice.Infrastructure.Persistence.Models;

namespace BackOffice.Domain.Patients
{
    public interface IPatientRepository
    {
        Task<PatientDataModel> AddAsync(Patient patient);
        Task<PatientDataModel> GetByIdAsync(RecordNumber id);
        Task<List<PatientDataModel>> GetAllAsync();
        Task UpdateAsync(Patient patient);
        Task DeleteAsync(RecordNumber id);
    }
}
