using BackOffice.Application.Specialization;
using BackOffice.Infraestructure.Specialization;

namespace BackOffice.Domain.Specialization
{
    public interface ISpecializationRepository
    {
        
        
            Task<SpecializationsDataModel> AddAsync(Domain.Specialization.Specializations specialization);
            Task<SpecializationsDataModel> GetByIdAsync(Specializations id);
            Task<List<SpecializationsDataModel>> GetAllAsync();
            Task UpdateAsync(Domain.Specialization.Specializations specialization);
            Task DeleteAsync(Application.Specialization.SpecializationDto id);
            Task<IEnumerable<SpecializationDto>> GetFilteredAsync();
        

    }
}
