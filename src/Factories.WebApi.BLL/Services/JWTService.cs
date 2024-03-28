using Factories.WebApi.BLL.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Factories.WebApi.BLL.Services
{
    public class JwtService(string issuer, string audience, string key)
    {
        private readonly string _key = key;

        public string GenerateJwtToken(IdentityUser user, IList<Claim> additionalClaims, IList<string> roles)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_key);


            var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Name, user.UserName),
        };

            if (additionalClaims != null && additionalClaims.Any())
                claims.AddRange(additionalClaims);

            if (roles != null && roles.Any())
                foreach (var role in roles)
                    claims.Add(new Claim(ClaimTypes.Role, role.ToString()));


            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(2),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}