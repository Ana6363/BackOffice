using BackOffice.Application.OperationTypes;
using BackOffice.Domain.OperationType;
using BackOffice.Domain.Staff;
using BackOffice.Infrastructure;
using BackOffice.Infrastructure.OperationTypes;
using Microsoft.EntityFrameworkCore;


namespace BackOffice.Infraestructure.OperationTypes
{
    public class OperationTypeRepository : IOperationTypeRepository
    {
        private readonly BackOfficeDbContext _context;

        public OperationTypeRepository(BackOfficeDbContext context)
        {
            _context = context;
        }

        public async Task<OperationTypeDataModel> AddAsync(OperationTypeDataModel operationType)
        {
            ArgumentNullException.ThrowIfNull(operationType);
            var operationTypeDataModel = new OperationTypeDataModel
            {
                OperationTypeId = Guid.NewGuid().ToString(), 
                PreparationTime = operationType.PreparationTime,
                SurgeryTime = operationType.SurgeryTime,
                CleaningTime = operationType.CleaningTime,
                OperationTypeName = operationType.OperationTypeName,
                Specializations = operationType.Specializations.Select(s => new OpTypeRequirementsDataModel
                {
                    SpecializationId = Guid.NewGuid().ToString(), 
                    Name = s.Name,
                    NeededPersonnel = s.NeededPersonnel, 
                    OperationTypeId = operationType.OperationTypeId
                }).ToList()
            };

            await _context.OperationType.AddAsync(operationTypeDataModel);
            await _context.SaveChangesAsync();

            return operationTypeDataModel;
        }

        public async Task<OperationTypeDataModel> GetByIdAsync (string id)
        {
            var operationId = id;

            var operationType = await _context.OperationType
                .Include(o => o.Specializations)
                .FirstOrDefaultAsync(o => o.OperationTypeId == operationId) ?? null;
            return operationType;
        }
        
        
        public async Task<List<OperationTypeDataModel>> GetAllAsync()
        {
            return await _context.OperationType
            .Include(o => o.Specializations)
            .ToListAsync();
        }
       
         public async Task UpdateAsync(OperationTypeDataModel operationTypeDataModel)
        {
            var existingEntry = await _context.OperationType
                .Include(o => o.Specializations) 
                .FirstOrDefaultAsync(o => o.OperationTypeId == operationTypeDataModel.OperationTypeId);

            if (existingEntry == null)
            {
                throw new InvalidOperationException("The operation type with the specified ID does not exist.");
            }

            existingEntry.PreparationTime = operationTypeDataModel.PreparationTime;
            existingEntry.SurgeryTime = operationTypeDataModel.SurgeryTime;
            existingEntry.CleaningTime = operationTypeDataModel.CleaningTime;
            existingEntry.OperationTypeName = operationTypeDataModel.OperationTypeName;

            // Update Specializations
            if (operationTypeDataModel.Specializations != null && operationTypeDataModel.Specializations.Any())
            {
                _context.OperationRequirements.RemoveRange(existingEntry.Specializations);

                var newSpecializations = operationTypeDataModel.Specializations.Select(s => new OpTypeRequirementsDataModel
                {
                    SpecializationId = Guid.NewGuid().ToString(), //generate new ID if there is no ID
                    Name = s.Name,
                    NeededPersonnel = s.NeededPersonnel,
                    OperationTypeId = existingEntry.OperationTypeId
                }).ToList();

                existingEntry.Specializations = newSpecializations;
            }

            await _context.SaveChangesAsync();
        }


        public async Task DeleteAsync (string name)
        {
            var operationTypeDataModel = await GetByNameAsync(name);
            if (operationTypeDataModel != null)
            {
                _context.OperationType.Remove(operationTypeDataModel);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<OperationTypeDataModel> GetByNameAsync (string name)
        {
            var operationName = name;

            var operationType = await _context.OperationType
                .Include(o => o.Specializations)
                .FirstOrDefaultAsync(o => o.OperationTypeName == operationName) ?? null;
            return operationType;
        }

        public async Task<string> GetMaxOperationTypeIdAsync()
        {
            return await _context.OperationType
                .MaxAsync(o => o.OperationTypeId);
        }

        
    }
}