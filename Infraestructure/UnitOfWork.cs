using System.Threading.Tasks;
using BackOffice.Domain.Shared;

namespace BackOffice.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly BackOfficeDbContext _context;

        public UnitOfWork(BackOfficeDbContext context)
        {
            this._context = context;
        }

        public async Task<int> CommitAsync()
        {
            return await this._context.SaveChangesAsync();
        }
    }
}