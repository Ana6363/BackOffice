using BackOffice.Domain.Shared;

namespace BackOffice.Domain.Users
{
    public interface IUserRepository : IRepository<User, UserId>
    {
        Task UpdateAsync(User user);
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByPhoneNumberAsync(int? number);
        Task DeleteAsync(UserId user);
    }
}
