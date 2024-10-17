using Google.Apis.Auth;
using Google.Apis.Auth.OAuth2.Responses;

namespace BackOffice.Application.OAuth
{
    public interface IGoogleOAuthService
    {
        Task<TokenResponse> ExchangeCodeForTokensAsync(string code);
        Task<GoogleJsonWebSignature.Payload> ValidateToken(string idToken);
    }
}
