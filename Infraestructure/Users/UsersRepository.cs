using BackOffice.Domain.Users;
using Microsoft.EntityFrameworkCore;
using BackOffice.Domain.Shared;
using BackOffice.Application.Users;
using BackOffice.Infrastructure;
using BackOffice.Infrastructure.Persistence.Models;

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
    var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == domainModel.Id.AsString());

    if (existingUser != null)
    {
        _context.Entry(existingUser).State = EntityState.Detached;

        var newUser = new UserDataModel
        {
            Id = domainModel.Id.Value,
            Role = domainModel.Role,
            Active = domainModel.Active,
            PhoneNumber = domainModel.PhoneNumber.Number,
            ActivationToken = domainModel.ActivationToken,
            TokenExpiration = domainModel.TokenExpiration,
            FirstName = domainModel.FirstName.NameValue,
            LastName = domainModel.LastName.NameValue,
            FullName = domainModel.FullName.NameValue,
            IsToBeDeleted = domainModel.IsToBeDeleted
        };

        _context.Users.Remove(existingUser);
        _context.Users.Add(newUser);
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

    public async Task<User?> GetByPhoneNumberAsync(int? number)
    {
        var dataModel = await _context.Users.FirstOrDefaultAsync(u => u.PhoneNumber == number);
        return dataModel == null ? null : UserMapper.ToDomain(dataModel);
    }

    public Task<bool> FindAsync(UserId id)
    {
        throw new NotImplementedException();
    }

     public async Task DeleteAsync(UserId userId)
{
    var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId.AsString());

    if (user == null)
    {
        throw new Exception("User not found.");
    }
    _context.Users.Remove(user);
    await _context.SaveChangesAsync();
}


}
