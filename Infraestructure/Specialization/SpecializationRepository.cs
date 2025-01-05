using BackOffice.Application.Specialization;
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

        public async Task<SpecializationsDataModel> AddAsync(Domain.Specialization.Specialization specialization)
        {
            if (specialization == null)
            {
                throw new ArgumentNullException(nameof(specialization), "Specialization cannot be null.");
            }
            Console.WriteLine(specialization.Id.AsString());
            Console.WriteLine(specialization.Description.ToString());
            var specializationDataModel = new SpecializationsDataModel
            {
                Id = specialization.Id.AsString(),
                Description = specialization.Description.ToString()
            };
            Console.WriteLine(specializationDataModel.Id);
            Console.WriteLine(specializationDataModel.Description);
            await _context.Specializations.AddAsync(specializationDataModel);
            await _context.SaveChangesAsync();
            return specializationDataModel;
        }

        public async Task<SpecializationsDataModel?> GetByIdAsync(Specializations id)
        {
            var specializationString = id.AsString();
            return await _context.Specializations
                .FirstOrDefaultAsync(s => s.Id == specializationString);
        }

        public async Task<List<SpecializationsDataModel>> GetAllAsync()
        {
            return await _context.Specializations.ToListAsync();
        }

        public async Task UpdateAsync(Domain.Specialization.Specialization specialization)
        {
            var specializationDataModel = await GetByIdAsync(specialization.Id);
            if (specializationDataModel == null)
            {
                throw new Exception("Specialization not found.");
            }
            specializationDataModel.Description = specialization.Description.ToString();
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(SpecializationDto specializationDto)
        {
            if (specializationDto == null || string.IsNullOrWhiteSpace(specializationDto.Name))
            {
                throw new ArgumentException("SpecializationDto or Name cannot be null or empty.");
            }
            var specializationDataModel = await _context.Specializations
                .FirstOrDefaultAsync(s => s.Id == specializationDto.Name);

            if (specializationDataModel == null)
            {
                throw new InvalidOperationException("Specialization not found.");
            }

            _context.Specializations.Remove(specializationDataModel);
            await _context.SaveChangesAsync();
        }

        
    }
}
