using ADAuthenticatorApi.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ADAuthenticatorApi.Services
{
    public class JwtTokenService
    {
        private readonly JwtSettings _config;

        public JwtTokenService(IOptions<JwtSettings> options)
        {
            _config = options.Value;
        }

        public string GenerateToken(string username, IDictionary<string, string> attributes)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.Key!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username)
            };

            if (attributes != null)
            {
                if (attributes.TryGetValue("employeeID", out var empId))
                    claims.Add(new Claim("Personnelid", empId));

                if (attributes.TryGetValue("email", out var email))
                    claims.Add(new Claim(ClaimTypes.Email, email));

                if (attributes.TryGetValue("displayName", out var name))
                    claims.Add(new Claim(ClaimTypes.GivenName, name));
            }

            var token = new JwtSecurityToken(
                issuer: _config.Issuer,
                audience: _config.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_config.ExpireMinutes!),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
