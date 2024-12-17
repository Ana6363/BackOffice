using BackOffice.Domain.Shared;
using BackOffice.Domain.Specialization;
using BackOffice.Infraestructure.Specialization;
using BackOffice.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace BackOffice.Application.Specialization
{
    public class SpecializationService
    {
        private readonly ISpecializationRepository _specializationRepository;
        private readonly BackOfficeDbContext _dbContext;
        private readonly IUnitOfWork _unitOfWork;

        public SpecializationService(ISpecializationRepository specializationRepository, BackOfficeDbContext dbContext, IUnitOfWork unitOfWork)
        {
            _specializationRepository = specializationRepository;
            _dbContext = dbContext;
            _unitOfWork = unitOfWork;
        }

        public async Task<SpecializationDto> AddAsync(SpecializationDto specializationDto)
        {
            var specialization = SpecializationMapper.ToDomain(specializationDto);

            var existingSpecialization = await _specializationRepository.GetByIdAsync(specialization.Id);

            if (existingSpecialization != null)
            {
                throw new ArgumentException("Specialization already exists", nameof(specializationDto));
            }

            try
            {
                await _specializationRepository.AddAsync(specialization);
                await _unitOfWork.CommitAsync();
                return SpecializationMapper.ToDto(specialization);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while adding the specialization", ex);
            }
        }

        public async Task<SpecializationDto> UpdateAsync(SpecializationDto specializationDto)
        {
            var specialization = SpecializationMapper.ToDomain(specializationDto);
            var existingSpecialization = await _specializationRepository.GetByIdAsync(specialization.Id);
            if (existingSpecialization == null)
            {
                throw new ArgumentException("Specialization does not exist", nameof(specializationDto));
            }
            try
            {
                await _specializationRepository.UpdateAsync(specialization);
                await _unitOfWork.CommitAsync();
                return SpecializationMapper.ToDto(specialization);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the specialization", ex);
            }
        }

        public async Task<SpecializationDto> GetByIdAsync(Specializations id)
        {
            var specialization = await _specializationRepository.GetByIdAsync(id);
            if (specialization == null)
            {
                throw new ArgumentException("Specialization does not exist", nameof(id));
            }

            var specializationForDto = SpecializationMapper.ToDomain(specialization);
            return SpecializationMapper.ToDto(specializationForDto);
        }

        public async Task<IEnumerable<SpecializationsDataModel>> GetFilteredAsync(SpecializationFilterDto specializationFilter)
        {
            var query = from specialization in _dbContext.specializationsDataModels
                        select specialization;

            if (!string.IsNullOrWhiteSpace(specializationFilter.Name))
            {
                query = query.Where(x => x.Id.Contains(specializationFilter.Name));
            }

            if (!string.IsNullOrWhiteSpace(specializationFilter.Description))
            {
                query = query.Where(x => x.Description.Contains(specializationFilter.Description));
            }

            var result = await query.ToListAsync();

            return result;
        }

        public async Task DeleteAsync(SpecializationDto id)
        {
            var specialization = await _dbContext.specializationsDataModels.FirstOrDefaultAsync(x => x.Id.Equals(id.Name.ToString()));
            if (specialization == null)
            {
                throw new ArgumentException("Specialization does not exist", nameof(id));
            }
            try
            {
                await _specializationRepository.DeleteAsync(id);
                await _unitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the specialization", ex);
            }
        }


    }
}
