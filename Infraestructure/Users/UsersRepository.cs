using BackOffice.Domain.Users;
using Microsoft.EntityFrameworkCore;
using BackOffice.Domain.Shared;
using BackOffice.Application.Users;
using BackOffice.Infrastructure;

public class UsersRepository : IUserRepository
{
    private readonly BackOfficeDbContext _context;

    public UsersRepository(BackOfficeDbContext context)
    {
        _context = context;
    }

    public async Task<List<User>> GetAllAsync()
    {
        var dataModels = await _context.Users.ToListAsync();
        return dataModels.Select(dataModel => UserMapper.ToDomain(dataModel)).ToList();
    }

    public async Task<User?> GetByIdAsync(UserId id)
    {
        var dataModel = await _context.Users.FirstOrDefaultAsync(u => u.Id == id.AsString());
        return dataModel == null ? null : UserMapper.ToDomain(dataModel);
    }

    public async Task AddAsync(User user)
    {
        var dataModel = UserMapper.ToDataModel(user);
        await _context.Users.AddAsync(dataModel);
        await _context.SaveChangesAsync();
    }

    async Task<User> IRepository<User, UserId>.AddAsync(User obj)
    {
        var dataModel = UserMapper.ToDataModel(obj);
        await _context.Users.AddAsync(dataModel);
        await _context.SaveChangesAsync();
        return obj;
    }

    public async Task UpdateAsync(User domainModel)
    {
        var dataModel = await _context.Users.FirstOrDefaultAsync(u => u.Id == domainModel.Id.AsString());
        if (dataModel != null)
        {
            dataModel.Role = domainModel.Role;
            dataModel.Active = domainModel.Active;
            dataModel.ActivationToken = domainModel.ActivationToken;
            dataModel.TokenExpiration = domainModel.TokenExpiration;

            dataModel.FirstName = domainModel.FirstName.NameValue;
            dataModel.LastName = domainModel.LastName.NameValue;
            dataModel.FullName = domainModel.FullName.NameValue;
            dataModel.IsToBeDeleted = domainModel.IsToBeDeleted;

            _context.Users.Update(dataModel);
            await _context.SaveChangesAsync();
        }
    }

    public void Delete(User domainModel)
    {
        var dataModel = _context.Users.FirstOrDefault(u => u.Id == domainModel.Id.AsString());
        if (dataModel != null)
        {
            _context.Users.Remove(dataModel);
            _context.SaveChanges();
        }
    }

    public Task<List<User>> GetByIdsAsync(List<UserId> ids)
    {
        throw new NotImplementedException();
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        var dataModel = await _context.Users.FirstOrDefaultAsync(u => u.Id == email);
        return dataModel == null ? null : UserMapper.ToDomain(dataModel);
    }

    public Task<bool> FindAsync(UserId id)
    {
        throw new NotImplementedException();
    }
}
