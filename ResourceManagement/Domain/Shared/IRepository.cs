namespace BackOffice.ResourceManagement.Domain.Shared
{
    public interface IRepository<TEntity, TEntityId>
    {
        Task<List<TEntity>> GetAll();
        Task<TEntity> GetById(TEntityId id);

        //Task<List<TEntity>> GetByIdsAsync(List<TEntityId> ids);
        Task<TEntity> Add(TEntity obj);
        void Delete(TEntity obj);
        Task<bool> Find(TEntityId id);
    }
}