using MedSchedule.Domain.Services;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MedSchedule.Infrastructure.Services
{
    public class TokenService : ITokenService
    {
        private readonly string _signKey;
        private readonly int _expiresAt;

        public TokenService(string signKey, int expiresAt)
        {
            _signKey = signKey;
            _expiresAt = expiresAt;
        }

        public string GenerateToken(List<Claim> claims, Guid userId)
        {
            claims.Add(new Claim(ClaimTypes.Sid, userId.ToString()));

            var descriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_expiresAt),
                SigningCredentials = new SigningCredentials(GetSecurityKey(), SecurityAlgorithms.HmacSha256Signature)
            };

            var handler = new JwtSecurityTokenHandler();

            var create = handler.CreateToken(descriptor);

            return handler.WriteToken(create);
        }



        private SymmetricSecurityKey GetSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_signKey));
        }
    }
}
