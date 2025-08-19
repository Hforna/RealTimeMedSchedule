using MedSchedule.Domain.Aggregates.UserAggregate;
using MedSchedule.Domain.Exceptions;
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
        public DateTime GenerateRefreshExpiration() => DateTime.UtcNow.AddHours(_expiresAt);
        public string GenerateRefreshToken() => Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        public string GenerateToken(List<Claim> claims, Guid userId)
        {
            claims.Add(new Claim(ClaimTypes.Sid, userId.ToString()));

            claims.Add(new Claim(
            JwtRegisteredClaimNames.Exp,
            new DateTimeOffset(DateTime.UtcNow.AddMinutes(_expiresAt)).ToUnixTimeSeconds().ToString(),
            ClaimValueTypes.Integer64
            ));


            var descriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                SigningCredentials = new SigningCredentials(GetSecurityKey(), SecurityAlgorithms.HmacSha256Signature),
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

            var validatorParameters = new TokenValidationParameters()
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = GetSecurityKey(),
                ValidateIssuer = false,
                ValidateAudience = false,
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var principal = tokenHandler.ValidateToken(token, validatorParameters, out var securityToken);

            var uid = Guid.Parse(principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Sid)?.Value!);
            var user = await _userRepository.GetUserById(uid);
            return user;
        }

        private SymmetricSecurityKey GetSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_signKey));
        }
    }
}
