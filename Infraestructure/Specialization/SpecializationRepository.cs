using BackOffice.Domain.Specialization;
using BackOffice.Infrastructure;
using Microsoft.EntityFrameworkCore;
using BackOffice.Application.Specialization;

namespace BackOffice.Infraestructure.Specialization
{
    public class SpecializationRepository : ISpecializationRepository
    {
        private readonly BackOfficeDbContext _context;

        public SpecializationRepository(BackOfficeDbContext context)
        {
            _context = context;
        }

        // Implementação corrigida - usa "Specialization" como tipo do parâmetro
        public async Task<SpecializationsDataModel> AddAsync(Domain.Specialization.Specializations specialization)
        {
            if (specialization == null)
            {
                throw new ArgumentNullException(nameof(specialization), "Specialization cannot be null.");
            }
            var specializationDataModel = new SpecializationsDataModel
            {
                Id = specialization.AsString(),
                Description = "Default Description" // Exemplo
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

        // Corrigido - parâmetro "Specialization" corresponde à interface
        public async Task UpdateAsync(Domain.Specialization.Specializations specialization)
        {
            var specializationDataModel = await GetByIdAsync(new Specializations(specialization.AsString()));
            if (specializationDataModel == null)
            {
                throw new Exception("Specialization not found.");
            }
            specializationDataModel.Id = specialization.AsString();
            await _context.SaveChangesAsync();
        }

        // Corrigido - parâmetro "SpecializationDto" corresponde à interface
        public async Task DeleteAsync(Application.Specialization.SpecializationDto id)
        {
            var specializationDataModel = await _context.specializationsDataModels
                .FirstOrDefaultAsync(s => s.Id == id.Name);
            if (specializationDataModel == null)
            {
                throw new Exception("Specialization not found.");
            }
            _context.specializationsDataModels.Remove(specializationDataModel);
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<SpecializationDto>> GetFilteredAsync()
        {
            return await _context.specializationsDataModels
                .Select(s => new SpecializationDto(s.Id, s.Description))
                .ToListAsync();
        }
    }
}
