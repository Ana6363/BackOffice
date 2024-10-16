using System.Threading.Tasks;
using System.Collections.Generic;
using BackOffice.Domain.Shared;

namespace BackOffice.Domain.Users
{
    public class UserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _repo;

        public UserService(IUnitOfWork unitOfWork, IUserRepository repo)
        {
            this._unitOfWork = unitOfWork;
            this._repo = repo;
        }

        public async Task<List<UserDto>> GetAllAsync()
        {
            var list = await this._repo.GetAllAsync();
            
            List<UserDto> listDto = list.ConvertAll<UserDto>(user => new UserDto{Id = user.Id.AsString(), Role = user.Role});

            return listDto;
        }

        public async Task<UserDto?> GetByIdAsync(UserId id)
        {
            var user = await this._repo.GetByIdAsync(id);
            
            if(user == null)
                return null;

            return new UserDto{Id = user.Id.AsString(), Role = user.Role};
        }

        public async Task<UserDto> AddAsync(UserDto dto)
        { 
            var user = new User(dto.Id, dto.Role);

            await this._repo.AddAsync(user);

            await this._unitOfWork.CommitAsync();

            return new UserDto { Id = user.Id.AsString(), Role = user.Role };
        }

        public async Task<UserDto?> UpdateAsync(UserDto dto)
        {
            var user = await this._repo.GetByIdAsync(new UserId(dto.Id)); 

            if (user == null)
                return null;   

            // change all field
            user.ChangeRole(dto.Role);
            
            await this._unitOfWork.CommitAsync();

            return new UserDto { Id = user.Id.AsString(), Role = user.Role };
        }

        public async Task<UserDto> InactivateAsync(UserId id)
        {
            var user = await this._repo.GetByIdAsync(id); 

            if (user == null)
                return null;   

            // change all fields
            user.MarkAsInactive();
            
            await this._unitOfWork.CommitAsync();

            return new UserDto { Id = user.Id.AsString(), Role = user.Role };
        }

         public async Task<UserDto?> DeleteAsync(UserId id)
        {
            var user = await this._repo.GetByIdAsync(id); 

            if (user == null)
                return null;   

            if (user.Active)
                throw new BusinessRuleValidationException("It is not possible to delete an active user.");
            
            this._repo.Delete(user);
            await this._unitOfWork.CommitAsync();

            return new UserDto { Id = user.Id.AsString(), Role = user.Role };
        }
    }
}