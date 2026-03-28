using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Auth
{
    public class JwtHelper
    {
        public static string GenerateJwtToken(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, name),
                new Claim(ClaimTypes.Role, "Admin")
            };

            var credentials = new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken("myserver","myclients",claims, expires: DateTime.UtcNow.AddHours(1), signingCredentials: credentials);
            return JwtSecurityTokenHandler.WriteToken(token);
        }

        public static readonly JwtSecurityTokenHandler JwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        public static readonly SymmetricSecurityKey SecurityKey = new SymmetricSecurityKey(Convert.FromBase64String("Qm8xY2pZV3p1R2lYc2p0cU5Vd3RZcG5qV0ZrM1p6Z0tYb1d6Y2pRZw=="));

    }
}
