using System.Threading.Tasks;
using System.Collections.Generic;
using BackOffice.Domain.Shared;
using BackOffice.Application.Services;

namespace BackOffice.Domain.Users
{
    public class UserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _repo;
        private readonly IEmailService _emailService;

        public UserService(IUnitOfWork unitOfWork, IUserRepository repo, IEmailService emailService)
        {
            this._unitOfWork = unitOfWork;
            this._repo = repo;
            this._emailService = emailService; 
        }

        public async Task<List<UserDto>> GetAllAsync()
        {
            var list = await this._repo.GetAllAsync();
            return list.ConvertAll(UserMapper.ToDto);
        }

        public async Task<UserDto?> GetByIdAsync(UserId id)
        {
            var user = await this._repo.GetByIdAsync(id);
            return user == null ? null : UserMapper.ToDto(user);
        }

      public async Task<UserDto> AddAsync(UserDto dto)
        {
            var user = UserMapper.ToDomain(dto);

            if (_repo == null)
            {
                throw new Exception("User repository is not initialized.");
            }

            // Check if the user already exists
            var existingUser = await _repo.GetByIdAsync(new UserId(dto.Id));
            if (existingUser != null)
            {
                throw new Exception("User already exists in the database.");
            }

            Console.WriteLine($"Adding user to repository: {user}");

            await this._repo.AddAsync(user);
            await this._unitOfWork.CommitAsync();

            return UserMapper.ToDto(user);
        }

        public async Task<string> GetUserRoleAsync(string email)
        {
            var user = await _repo.GetByEmailAsync(email);
            if (user == null)
            {
                throw new Exception("User not found.");
            }

            return user.Role; 
        }

        public async Task<UserDto?> UpdateAsync(UserDto dto)
        {
            var user = await this._repo.GetByIdAsync(new UserId(dto.Id)); 
            if (user == null)
                return null;

            user.ChangeRole(dto.Role);
            user.ActivationToken = dto.ActivationToken;
            user.TokenExpiration = dto.TokenExpiration;

            await this._repo.UpdateAsync(user);
            await this._unitOfWork.CommitAsync();

            return UserMapper.ToDto(user);
        }

        public async Task<UserDto> InactivateAsync(UserId id)
        {
            var user = await this._repo.GetByIdAsync(id); 
            if (user == null)
                return null;   

            user.MarkAsInactive();
            await this._unitOfWork.CommitAsync();
            return UserMapper.ToDto(user);
        }

        public async Task<UserDto?> DeleteAsync(UserId id)
        {
            var user = await this._repo.GetByIdAsync(id); 
            if (user == null)
                return null;   
            
            this._repo.Delete(user);
            await this._unitOfWork.CommitAsync();

            return UserMapper.ToDto(user);
        }
    }
}
