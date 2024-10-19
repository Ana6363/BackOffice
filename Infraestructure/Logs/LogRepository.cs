using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BackOffice.Application.Logs;
using BackOffice.Domain.Logs;
using BackOffice.Domain.Users;
using BackOffice.Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace BackOffice.Infrastructure.Persistence.Repositories
{
    public class LogRepository : ILogRepository
    {
        private readonly BackOfficeDbContext _context;

        public LogRepository(BackOfficeDbContext context)
        {
            _context = context;
        }

        public async Task<Log> AddAsync(Log log)
        {
            var dataModel = LogMapper.ToDataModel(log);
            await _context.Logs.AddAsync(dataModel);
            await _context.SaveChangesAsync();
            return log;
        }

        public async Task<List<Log>> GetAllAsync()
        {
            var dataModels = await _context.Logs.ToListAsync();
            return dataModels.Select(LogMapper.ToDomain).ToList();
        }

        public async Task<Log?> GetByIdAsync(LogId logId)
        {
            var dataModel = await _context.Logs.FirstOrDefaultAsync(l => l.LogId == logId.AsString());
            return dataModel == null ? null : LogMapper.ToDomain(dataModel);
        }

        public async Task<List<Log>> GetByUserEmailAsync(Email email)
        {
            var dataModels = await _context.Logs
                .Where(l => l.AffectedUserEmail == email.Value)
                .ToListAsync();
            return dataModels.Select(LogMapper.ToDomain).ToList();
        }

        public async Task DeleteAsync(Log log)
        {
            var dataModel = await _context.Logs.FirstOrDefaultAsync(l => l.LogId == log.Id.AsString());
            if (dataModel != null)
            {
                _context.Logs.Remove(dataModel);
                await _context.SaveChangesAsync();
            }
        }
    }
}
