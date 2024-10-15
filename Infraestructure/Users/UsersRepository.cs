using BackOffice.Domain.Users;
using BackOffice.Infrastructure.Shared;

namespace BackOffice.Infrastructure.Users
{
    public class UsersRepository : BaseRepository<User, UserId>, IUserRepository
    {
      
        public UsersRepository(BackOfficeDbContext context):base(context.Users)
        {
            
        }

    }
}