using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;

namespace BackOffice.Application.OAuth
{
    public class JwtTokenService
    {
        private readonly string _secretKey;
        private readonly string _issuer;
        private readonly string _audience;

        public JwtTokenService(IConfiguration configuration)
        {
            // Load values from appsettings.json
            _secretKey = configuration["Jwt:Key"];
            _issuer = configuration["Jwt:Issuer"];
            _audience = configuration["Jwt:Audience"];
        }

        public string GenerateToken(string email, string userRole)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Email, email),  // Include email claim
                new Claim(ClaimTypes.Role, userRole)  // Include role claim
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(20), // Set the expiration time
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
