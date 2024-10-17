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
using BackOffice.Application.Services;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly string _clientId = "1066153135876-ddrj4ms6r801m49cjun24ut057p95m18.apps.googleusercontent.com";
    private readonly string _clientSecret = "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"; 
    private readonly string _redirectUri = "http://localhost:5184/auth/callback";
    private readonly BackOfficeDbContext _dbContext;
    private readonly IEmailService _emailService;

    public AuthController(BackOfficeDbContext dbContext, IEmailService emailService)
    {
        _dbContext = dbContext;
        _emailService = emailService;
    }

    [HttpGet("login")]
    public IActionResult Login()
    {
        string authorizationUrl = $"https://accounts.google.com/o/oauth2/v2/auth?response_type=code&client_id={_clientId}&redirect_uri={_redirectUri}&scope=email&access_type=offline";
        return Redirect(authorizationUrl);
    }

    [HttpGet("callback")]
    public async Task<IActionResult> AuthCallback(string code)
    {
        try
        {
            var tokenResponse = await GetTokensAsync(code);
            if (tokenResponse == null)
            {
                return Unauthorized(new { success = false, message = "Failed to obtain tokens" });
            }

            var payload = await ValidateToken(tokenResponse.IdToken);
            if (payload == null || string.IsNullOrEmpty(payload.Email))
            {
                return Unauthorized(new { success = false, message = "Invalid token or email" });
            }

            var existingUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == new UserId(payload.Email));

            if (existingUser != null)
            {
                if (!existingUser.Active)
                {
                    if (!existingUser.IsActivationTokenValid() || string.IsNullOrEmpty(existingUser.ActivationToken))
                    {
                        existingUser.GenerateActivationToken();
                        await _dbContext.SaveChangesAsync();
                    }

                    await SendActivationEmail(existingUser);
                    return Unauthorized(new { success = false, message = "User is inactive. A new verification email has been sent." });
                }

                return Redirect("http://localhost:5184/swagger/index.html");
            }
            else
            {
                var newUser = new User(payload.Email, "Patient")
                {
                    Active = false
                };
                newUser.GenerateActivationToken();

                _dbContext.Users.Add(newUser);
                await _dbContext.SaveChangesAsync();

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
    public async Task<IActionResult> ActivateAccount(string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            return BadRequest(new { success = false, message = "Token is required" });
        }

        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.ActivationToken == token);
        if (user == null)
        {
            return NotFound(new { success = false, message = "Invalid token or user not found" });
        }

        if (user.Active)
        {
            return BadRequest(new { success = false, message = "User is already active" });
        }

        if (!user.IsActivationTokenValid())
        {
            return BadRequest(new { success = false, message = "Activation token has expired. Please request a new one." });
        }

        user.MarkAsActive();
        user.ActivationToken = null;
        user.TokenExpiration = null;
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
        string activationUrl = $"http://localhost:5184/auth/activate?token={user.ActivationToken}";
        string subject = "Activate Your Account";
        string body = $"Please activate within 24 hours your account by clicking on the following link: {activationUrl}";
        await _emailService.SendEmailAsync(user.Id.Value, subject, body);
    }
}
