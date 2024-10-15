using BackOffice.Domain.Shared;

namespace BackOffice.Domain.Users
{
    public interface IUserRepository:IRepository<User,UserId>
    {

    }
}