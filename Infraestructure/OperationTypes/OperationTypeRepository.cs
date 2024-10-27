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

        public async Task<OperationTypeDataModel> AddAsync(OperationType operationType)
        {
            ArgumentNullException.ThrowIfNull(operationType);
            var operationTypeDataModel = new OperationTypeDataModel
            {
                OperationTypeId = operationType.Id.AsString(),
                OperationTime = operationType.OperationTime.AsFloat(),
                OperationTypeName = operationType.OperationTypeName.Name,
                Specializations = operationType.Specializations.Select(s => new SpecializationDataModel
                {
                    SpecializationId = Guid.NewGuid().ToString(), 
                    Name = s.ToString(), 
                    OperationTypeId = operationType.Id.AsString() 
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
        public async Task UpdateAsync (OperationType operationType)
        {
            var operationTypeDataModel = await GetByIdAsync(operationType.Id.AsString());
            if (operationTypeDataModel == null)
            {
                throw new KeyNotFoundException($"OperationType with ID '{operationType.Id.AsString()}' not found.");
            }


            operationTypeDataModel.OperationTypeId = operationType.Id.AsString();
            operationTypeDataModel.OperationTypeName = operationType.OperationTypeName.Name;
            operationTypeDataModel.OperationTime = operationType.OperationTime.AsFloat();

            var existingSpecializations = await _context.Specializations
                .Where(s => s.OperationTypeId == operationTypeDataModel.OperationTypeId)
                .ToListAsync();
            // remove specializations from db that are not in received list fomr operationtype
            foreach (var existing in existingSpecializations)
            {
                if (!operationType.Specializations.Select(s => s.ToString()).Contains(existing.Name))
                {
                    _context.Specializations.Remove(existing); 
                }
            }
            foreach (var newSpecialization in operationType.Specializations)
            {
                if (!existingSpecializations.Any(s => s.Name == newSpecialization.ToString()))
                {
                    var newSpecializationDataModel = new SpecializationDataModel
                    {
                        SpecializationId = Guid.NewGuid().ToString(), 
                        Name = newSpecialization.ToString(),
                        OperationTypeId = operationTypeDataModel.OperationTypeId 
                    };

                    await _context.Specializations.AddAsync( newSpecializationDataModel); 
                }
            }
            //updates specializations list in operationTypeDataModel
            operationTypeDataModel.Specializations = await _context.Specializations
                .Where(s => s.OperationTypeId == operationTypeDataModel.OperationTypeId)
                .ToListAsync();

            _context.OperationType.Update(operationTypeDataModel);
            await _context.SaveChangesAsync();
        }



        public async Task DeleteAsync (string id)
        {
            var operationTypeDataModel = await GetByIdAsync(id);
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
                .FirstOrDefaultAsync(o => o.OperationTypeId == operationName) ?? null;
            return operationType;
        }

        public async Task<string> GetMaxOperationTypeIdAsync()
        {
            return await _context.OperationType
                .MaxAsync(o => o.OperationTypeId);
        }

        
    }
}