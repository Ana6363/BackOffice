using System;
using System.Threading.Tasks;
using BackOffice.Application.Services;
using BackOffice.Domain.Users;
using BackOffice.Infrastructure;
using Microsoft.EntityFrameworkCore;
using BackOffice.Application.OAuth;

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

        public async Task<(bool IsUserActive, string Message)> HandleOAuthCallbackAsync(string code)
        {
            var tokenResponse = await _googleOAuthService.ExchangeCodeForTokensAsync(code);

            var payload = await _googleOAuthService.ValidateToken(tokenResponse.IdToken);

            var existingUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == new UserId(payload.Email));

            if (existingUser != null)
            {
                if (!existingUser.Active)
                {
                    await SendActivationEmailAsync(existingUser.Id.Value);
                    return (false, "User is inactive. An activation email has been sent.");
                }

                return (true, "User is active");
            }
            else
            {
                // Register a new user
                var newUser = new User(payload.Email, "Patient")
                {
                    Active = false
                };
                newUser.GenerateActivationToken();
                _dbContext.Users.Add(newUser);
                await _dbContext.SaveChangesAsync();

                // Send activation email for new user
                await SendActivationEmailAsync(newUser.Id.Value);
                return (false, "User registered but inactive. An activation email has been sent.");
            }
        }

        public async Task ActivateUserAsync(string token)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.ActivationToken == token);

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
            await _dbContext.SaveChangesAsync();
        }

        public async Task SendActivationEmailAsync(string email)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == new UserId(email));
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
                await _dbContext.SaveChangesAsync();
            }

            string activationUrl = $"http://localhost:5184/auth/activate?token={user.ActivationToken}";
            string subject = "Activate Your Account";
            string body = $"Please activate your account by clicking on the following link: {activationUrl}";

            await _emailService.SendEmailAsync(email, subject, body);
        }
    }
}
