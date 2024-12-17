using BackOffice.Domain.Specialization;
using BackOffice.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace BackOffice.Infraestructure.Specialization
{
    public class SpecializationRepository : ISpecializationRepository
    {

        private readonly BackOfficeDbContext _context;

        public SpecializationRepository(BackOfficeDbContext context)
        {
            _context = context;
        }

        public async Task<SpecializationsDataModel> AddAsync(Specializations specialization)
        {
            if (specialization == null)
            {
                throw new ArgumentNullException(nameof(specialization), "Specialization cannot be null.");
            }
            var specializationDataModel = new SpecializationsDataModel
            {
                Id = specialization.AsString(),
                Description = "Default Description" // Set a default description or get it from specialization
            };
            await _context.specializationsDataModels.AddAsync(specializationDataModel);
            await _context.SaveChangesAsync();
            return specializationDataModel;
        }

        public async Task<SpecializationsDataModel?> GetByIdAsync(Specializations id)
        {
            var specializationString = id.AsString();
            return await _context.specializationsDataModels
                .FirstOrDefaultAsync(s => s.Id == specializationString);
        }

        public async Task<List<SpecializationsDataModel>> GetAllAsync()
        {
            return await _context.specializationsDataModels.ToListAsync();
        }

        public async Task UpdateAsync(Specializations specialization)
        {
            var specializationDataModel = await GetByIdAsync(specialization);
            if (specializationDataModel == null)
            {
                throw new Exception("Specialization not found."); // Handle not found case
            }
            specializationDataModel.Id = specialization.AsString();
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Specializations specialization)
        {
            var specializationDataModel = await GetByIdAsync(specialization);
            if (specializationDataModel == null)
            {
                throw new Exception("Specialization not found."); // Handle not found case
            }
            _context.specializationsDataModels.Remove(specializationDataModel);
            await _context.SaveChangesAsync();
        }

    }
}
