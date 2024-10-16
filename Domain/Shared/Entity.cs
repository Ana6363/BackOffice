
using Microsoft.EntityFrameworkCore;

namespace BackOffice.Domain.Shared
{
    public abstract class Entity<TEntityId>
    
    where TEntityId: EntityId
    {
         public TEntityId Id { get;  protected set; }

        internal void OnModelCreating(ModelBuilder modelBuilder)
        {
            throw new NotImplementedException();
        }
    }
}