

using Microsoft.AspNetCore.Mvc;
using Google.Apis.Auth;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System;
using BackOffice.Domain.Users;
using BackOffice.Infrastructure;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly string _clientId = "1066153135876-ddrj4ms6r801m49cjun24ut057p95m18.apps.googleusercontent.com";
    private readonly string _clientSecret = "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"; // Set this value securely in production
    private readonly string _redirectUri = "http://localhost:5184/auth/callback";
    private readonly BackOfficeDbContext _dbContext;
    private readonly IEmailService _emailService; // Assuming you have an email service

    // Inject the DbContext and Email Service into the controller
    public AuthController(BackOfficeDbContext dbContext, IEmailService emailService)
    {
        _dbContext = dbContext;
        _emailService = emailService;
    }
    [HttpGet("login")]
    public IActionResult Login()
    {
        // Redirect the user to Google's OAuth login page
        string authorizationUrl = $"https://accounts.google.com/o/oauth2/v2/auth?response_type=code&client_id={_clientId}&redirect_uri={_redirectUri}&scope=email&access_type=offline";
        return Redirect(authorizationUrl);
    }

    [HttpGet("callback")]
    public async Task<IActionResult> AuthCallback(string code)
    {
        try
        {
            // Exchange the authorization code for tokens
            var tokenResponse = await GetTokensAsync(code);

            if (tokenResponse == null)
            {
                return Unauthorized(new { success = false, message = "Failed to obtain tokens" });
            }

            // Validate the ID token and extract user email
            var payload = await ValidateToken(tokenResponse.IdToken);

            // Check if the token is valid and not expired
            if (payload == null || string.IsNullOrEmpty(payload.Email))
            {
                return Unauthorized(new { success = false, message = "Invalid token or email" });
            }

            // Check if the user exists in the database
            var existingUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == new UserId(payload.Email));

            if (existingUser != null)
            {
                // If the user exists, check if they are active
                if (!existingUser.Active)
                {
                    // If the user is inactive, send a verification email
                    await SendActivationEmail(existingUser);

                    return Unauthorized(new { success = false, message = "User is inactive. A verification email has been sent." });
                }
                
                // If the user is active, proceed normally (e.g., redirect to the main page)
                return Redirect("/");  // Replace "/" with your main page URL
            }
            else
        {
            // If the user is not found, register the user with Active = false
            var newUser = new User(payload.Email, "Patient") // Set default role if applicable
            {
                // You can initialize any other properties here
                Active = false
            };

            _dbContext.Users.Add(newUser);
            await _dbContext.SaveChangesAsync();

            // Send an activation email after registration
            await SendActivationEmail(newUser);

            return Unauthorized(new { success = false, message = "User registered but is inactive. A verification email has been sent." });
        }
    }
    catch (Exception ex)
    {
        return Unauthorized(new { success = false, message = ex.Message });
    }
}

    [HttpGet("activate")]
        public async Task<IActionResult> ActivateAccount(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest(new { success = false, message = "Email is required" });
            }

            // Find the user by email
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == new UserId(email));

            if (user == null)
            {
                return NotFound(new { success = false, message = "User not found" });
            }

            if (user.Active)
            {
                return BadRequest(new { success = false, message = "User is already active" });
            }

            // Activate the user
            user.MarkAsActive();

            // Save changes to the database
            await _dbContext.SaveChangesAsync();

            return Ok(new { success = true, message = "Account activated successfully" });
        }

    private async Task<TokenResponse> GetTokensAsync(string code)
    {
        var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
        {
            ClientSecrets = new ClientSecrets
            {
                ClientId = _clientId,
                ClientSecret = _clientSecret
            }
        });

        return await flow.ExchangeCodeForTokenAsync("user", code, _redirectUri, CancellationToken.None);
    }

    private async Task<GoogleJsonWebSignature.Payload> ValidateToken(string idToken)
    {
        var settings = new GoogleJsonWebSignature.ValidationSettings
        {
            Audience = new[] { _clientId }
        };

        return await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
    }

    private async Task SendActivationEmail(User user)
    {
        // Assuming you have a method to generate the activation URL
        string activationUrl = $"http://localhost:5184/auth/activate?email={user.Id.Value}";

        string subject = "Activate Your Account";
        string body = $"Please activate your account by clicking on the following link: {activationUrl}";

        // Use your email service to send the email
        await _emailService.SendEmailAsync(user.Id.Value, subject, body);
    }
}