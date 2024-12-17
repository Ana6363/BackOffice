using BackOffice.Application.Specialization;
using BackOffice.Infraestructure.Specialization;

namespace BackOffice.Domain.Specialization
{
    public interface ISpecializationRepository
    {
        
        
            Task<SpecializationsDataModel> AddAsync(Domain.Specialization.Specialization specialization);
            Task<SpecializationsDataModel> GetByIdAsync(Specializations id);
            Task<List<SpecializationsDataModel>> GetAllAsync();
            Task UpdateAsync(Specialization specialization);
            Task DeleteAsync(Specializations id);
        

    }
}
