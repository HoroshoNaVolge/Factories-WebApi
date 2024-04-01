using Factories.WebApi.BLL.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Factories.WebApi.BLL.Services
{
    public interface IJwtService
    {
        string GenerateJwtToken(IdentityUser user, IList<Claim> additionalClaims, IList<string> roles);
    }

    public class JwtService(IOptions<JwtConfig> jwtConfig) : IJwtService
    {
        private readonly IOptions<JwtConfig> jwtConfig = jwtConfig;
        public string GenerateJwtToken(IdentityUser user, IList<Claim> additionalClaims, IList<string> roles)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(jwtConfig.Value.SecretKey);

            var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email!),
            new(ClaimTypes.Name, user.UserName!),
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
                Issuer = jwtConfig.Value.Issuer,
                Audience = jwtConfig.Value.Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
