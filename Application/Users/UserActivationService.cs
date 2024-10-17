using System;
using System.Threading.Tasks;
using BackOffice.Application.Services;
using BackOffice.Application.OAuth;
using BackOffice.Domain.Users;
using BackOffice.Infrastructure;
using BackOffice.Application.Users;
using Microsoft.EntityFrameworkCore;

namespace BackOffice.Application.Users
{
    public class UserActivationService
    {
        private readonly BackOfficeDbContext _dbContext;
        private readonly IEmailService _emailService;
        private readonly IGoogleOAuthService _googleOAuthService;

        public UserActivationService(BackOfficeDbContext dbContext, IEmailService emailService, IGoogleOAuthService googleOAuthService)
        {
            _dbContext = dbContext;
            _emailService = emailService;
            _googleOAuthService = googleOAuthService;
        }

        public async Task<(bool IsUserActive, string Role, string Message)> HandleOAuthCallbackAsync(string code)
{
    var tokenResponse = await _googleOAuthService.ExchangeCodeForTokensAsync(code);
    var payload = await _googleOAuthService.ValidateToken(tokenResponse.IdToken);

    var userDataModel = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == payload.Email);
    var existingUser = userDataModel != null ? UserMapper.ToDomain(userDataModel) : null;

    if (existingUser != null)
    {
        if (!existingUser.Active)
        {
            await SendActivationEmailAsync(existingUser.Id.AsString());
            return (false, null, "User is inactive. An activation email has been sent.");
        }

        return (true, existingUser.Role, "User is active");
    }
    else
    {
        var newUser = new User(payload.Email, "Patient") { Active = false };
        newUser.GenerateActivationToken();

        var newUserDataModel = UserMapper.ToDataModel(newUser);
        _dbContext.Users.Add(newUserDataModel);
        await _dbContext.SaveChangesAsync();

        await SendActivationEmailAsync(newUser.Id.AsString());
        return (false, null, "User registered but inactive. An activation email has been sent.");
    }
}

        public async Task ActivateUserAsync(string token)
        {
            // Query for the user data model by token
            var userDataModel = await _dbContext.Users.FirstOrDefaultAsync(u => u.ActivationToken == token);
            var user = userDataModel != null ? UserMapper.ToDomain(userDataModel) : null;

            if (user == null)
            {
                throw new Exception("Invalid token or user not found.");
            }

            if (user.Active)
            {
                throw new Exception("User is already active.");
            }

            if (!user.IsActivationTokenValid())
            {
                throw new Exception("Activation token has expired. Please request a new one.");
            }

            user.MarkAsActive();
            user.ActivationToken = null;
            user.TokenExpiration = null;

            var updatedUserDataModel = UserMapper.ToDataModel(user);
            _dbContext.Entry(userDataModel).CurrentValues.SetValues(updatedUserDataModel);
            await _dbContext.SaveChangesAsync();
        }


        public async Task<UserDto> RegisterUserAsync(string email, string role)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(role))
            {
                throw new ArgumentException("Email and role must be provided.");
            }

            // Check if the user already exists
            var existingUserDataModel = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == email);
            if (existingUserDataModel != null)
            {
                throw new Exception("User already exists.");
            }

            // Create new user
            var newUser = new User(email, role)
            {
                Active = false // User is initially inactive
            };

            // Generate activation token
            newUser.GenerateActivationToken();

            // Save user to the database
            var newUserDataModel = UserMapper.ToDataModel(newUser);
            _dbContext.Users.Add(newUserDataModel);
            await _dbContext.SaveChangesAsync();

            // Send activation email using the existing method
            await SendActivationEmailAsync(email);

            // Return the UserDto representation of the newly created user
            return UserMapper.ToDto(newUser);
        }
        
        public async Task SendActivationEmailAsync(string email)
        {
            var userDataModel = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == email);
            var user = userDataModel != null ? UserMapper.ToDomain(userDataModel) : null;

            if (user == null)
            {
                throw new Exception("User not found.");
            }

            if (user.Active)
            {
                throw new Exception("User is already active.");
            }

            if (!user.IsActivationTokenValid() || string.IsNullOrEmpty(user.ActivationToken))
            {
                user.GenerateActivationToken();

                var updatedUserDataModel = UserMapper.ToDataModel(user);
                _dbContext.Entry(userDataModel).CurrentValues.SetValues(updatedUserDataModel);
                await _dbContext.SaveChangesAsync();
            }

            // Send email
            string activationUrl = $"http://localhost:5184/auth/activate?token={user.ActivationToken}";
            string subject = "Activate Your Account";
            string body = $"Please activate your account by clicking on the following link: {activationUrl}";

            await _emailService.SendEmailAsync(email, subject, body);
        }
    }
}
