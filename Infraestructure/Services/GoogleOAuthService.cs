using BackOffice.Application.OAuth;
using Google.Apis.Auth;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using System.Threading.Tasks;

namespace BackOffice.Infrastructure.Services  // You can move this to the Infrastructure folder
{
    public class GoogleOAuthService : IGoogleOAuthService  // Implement the interface here
    {
        private readonly string _clientId = "1066153135876-ddrj4ms6r801m49cjun24ut057p95m18.apps.googleusercontent.com";
        private readonly string _clientSecret = "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";
        private readonly string _redirectUri = "https://api-dotnet.hospitalz.site/api/v1/auth/callback";

        public async Task<TokenResponse> ExchangeCodeForTokensAsync(string code)
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

        public async Task<GoogleJsonWebSignature.Payload> ValidateToken(string idToken)
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[] { _clientId }
            };

            return await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
        }
    }
}
