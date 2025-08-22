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
        private readonly IUnitOfWork _uow;
        private readonly TokenValidationParameters _tokenValidation;

        public TokenService(string signKey, int expiresAt, 
            IRequestService requestService, IUnitOfWork uow, TokenValidationParameters tokenValidation)
        {
            _signKey = signKey;
            _expiresAt = expiresAt;
            _requestService = requestService;
            _uow = uow;
            _tokenValidation = tokenValidation;
        }

        public DateTime GenerateExpiration() => DateTime.UtcNow.AddMinutes(_expiresAt);
        public DateTime GenerateRefreshExpiration() => DateTime.UtcNow.AddHours(_expiresAt);
        public string GenerateRefreshToken() => Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        public string GenerateToken(List<Claim> claims, Guid userId)
        {
            claims.Add(new Claim(JwtRegisteredClaimNames.Sid, userId.ToString()));

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

            var principal = GetClaimsPrincipalFromToken(token);

            var uid = Guid.Parse(principal.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sid)!.Value!);
            var user = await _uow.UserRepository.GetUserById(uid);
            return user;
        }

        public Guid? GetUserGuidByToken()
        {
            var token = _requestService.GetBearerToken();

            if (string.IsNullOrEmpty(token))
                return null;

            var principal = GetClaimsPrincipalFromToken(token);

            var uid = Guid.Parse(principal.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sid)!.Value!);
            return uid;
        }

        private ClaimsPrincipal GetClaimsPrincipalFromToken(string token)
        {
            var validatorParameters = new TokenValidationParameters()
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = GetSecurityKey(),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(5)
            };

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            var tokenHandler = new JwtSecurityTokenHandler();

            return tokenHandler.ValidateToken(token, validatorParameters, out var securityToken);
        }

        private SymmetricSecurityKey GetSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_signKey));
        }
    }
}
