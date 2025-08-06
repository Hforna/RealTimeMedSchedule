using MedSchedule.Domain.Aggregates.UserAggregate;
using MedSchedule.Domain.Repositories;
using MedSchedule.Domain.Services;
using Microsoft.AspNetCore.Identity;
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
        private readonly IRequestService _requestService;
        private readonly IUserRepository _userRepository;

        public TokenService(string signKey, int expiresAt, IRequestService requestService, IUserRepository userRepository)
        {
            _signKey = signKey;
            _expiresAt = expiresAt;
            _requestService = requestService;
            _userRepository = userRepository;
        }

        public DateTime GenerateExpiration() => DateTime.UtcNow.AddMinutes(_expiresAt);

        public string GenerateRefreshToken() => Convert.ToBase64String(Guid.NewGuid().ToByteArray());
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

        public async Task<User?> GetUserByToken()
        {
            var token = _requestService.GetBearerToken();

            if (string.IsNullOrEmpty(token))
                return null;

            var handler = new JwtSecurityTokenHandler();

            var read = handler.ReadJwtToken(token);

            var userId = read.Claims.FirstOrDefault(d => d.Type == ClaimTypes.Sid)!.Value;
            var parse = Guid.Parse(userId);

            return await _userRepository.GetUserById(parse);
        }

        private SymmetricSecurityKey GetSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_signKey));
        }
    }
}
