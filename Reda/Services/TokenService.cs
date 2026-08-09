using Microsoft.IdentityModel.Tokens;
using Reda.Entities;
using Reda.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Reda.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;

        public TokenService(IConfiguration config)
        {
            _config = config;
        }

        public string CreateToken(User user)
        {
            // 1. تحديد البيانات اللي هتتخزن جوه التوكين (Claims)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role) // عشان نحدد لو كان Admin أو User
            };

            // 2. قراءة المفتاح السري وتشفيره (مكتوب في الـ appsettings)
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"] ?? "SuperSecretKey1234567890123456"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // 3. إعدادات التوكين (تاريخ الصلاحية والمصدر)
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(7), // التوكين هيفضل شغال أسبوع
                SigningCredentials = creds,
                Issuer = _config["Jwt:Issuer"],
                Audience = _config["Jwt:Audience"]
            };

            // 4. توليد التوكين وتحويله لنص صريح
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}