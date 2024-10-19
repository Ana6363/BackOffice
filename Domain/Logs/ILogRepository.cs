using System.Collections.Generic;
using System.Threading.Tasks;
using BackOffice.Domain.Logs;
using BackOffice.Domain.Users;

namespace BackOffice.Application.Logs
{
    public interface ILogRepository
    {
        Task<Log> AddAsync(Log log);
        Task<List<Log>> GetAllAsync();
        Task<Log?> GetByIdAsync(LogId logId);
        Task<List<Log>> GetByUserEmailAsync(Email email);
        Task DeleteAsync(Log log);
    }
}
