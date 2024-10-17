using BackOffice.Domain.Users;
using Microsoft.EntityFrameworkCore;
using BackOffice.Domain.Shared;
using BackOffice.Application.Users;

namespace BackOffice.Infrastructure.Users
{
    public class UsersRepository : IUserRepository
    {
        private readonly BackOfficeDbContext _context;

        public UsersRepository(BackOfficeDbContext context)
        {
            _context = context;
        }

        // Get all users, mapping from UserDataModel to the domain model (User)
        public async Task<List<User>> GetAllAsync()
        {
           var dataModels = await _context.Users.ToListAsync();
            return dataModels.Select(dataModel => UserMapper.ToDomain(dataModel)).ToList();

        }

        // Get user by ID, mapping from UserDataModel to the domain model (User)
        public async Task<User?> GetByIdAsync(UserId id)
        {
            var dataModel = await _context.Users.FirstOrDefaultAsync(u => u.Id == id.AsString());
            return dataModel == null ? null : UserMapper.ToDomain(dataModel);
        }

        // Add a user, mapping from the domain model to the data model (UserDataModel)
        public async Task AddAsync(User domainModel)
        {
            var dataModel = UserMapper.ToDataModel(domainModel);
            await _context.Users.AddAsync(dataModel);
            await _context.SaveChangesAsync();
        }

        // Update a user, mapping from the domain model to the data model (UserDataModel)
        public async Task UpdateAsync(User domainModel)
    {
        var dataModel = await _context.Users.FirstOrDefaultAsync(u => u.Id == domainModel.Id.AsString());
        if (dataModel != null)
        {
            // Update data model fields
            dataModel.Role = domainModel.Role;
            dataModel.Active = domainModel.Active;
            dataModel.ActivationToken = domainModel.ActivationToken;  // Ensure token is updated
            dataModel.TokenExpiration = domainModel.TokenExpiration;  // Ensure expiration is updated

            _context.Users.Update(dataModel);
            await _context.SaveChangesAsync();
        }
    }

        // Delete a user
        public void Delete(User domainModel)
        {
            var dataModel = _context.Users.FirstOrDefault(u => u.Id == domainModel.Id.AsString());
            if (dataModel != null)
            {
                _context.Users.Remove(dataModel);
            }
        }

        public Task<List<User>> GetByIdsAsync(List<UserId> ids)
        {
            throw new NotImplementedException();
        }

        Task<User> IRepository<User, UserId>.AddAsync(User obj)
        {
            throw new NotImplementedException();
        }

        public Task<bool> FindAsync(UserId id)
        {
            throw new NotImplementedException();
        }
    }
}
