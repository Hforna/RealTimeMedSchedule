using MedSchedule.Application.Requests;
using MedSchedule.Application.Responses;
using MedSchedule.Domain.Aggregates.UserAggregate;
using MedSchedule.Domain.AggregatesModel.UserAggregate;
using MedSchedule.Domain.Exceptions;
using MedSchedule.Domain.Repositories;
using MedSchedule.Domain.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MedSchedule.Application.Services
{
    public class LoginResponse
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpiresAt { get; set; }
    }

    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public interface ILoginService
    {
        public Task<LoginResponse> LoginByApplication(LoginRequest request);
    }

    public interface IUserService
    {
        public Task<UserResponse> Create(CreateUserRequest request);
    }

    public class LoginService : ILoginService
    {
        private readonly ILogger<LoginService> _logger;
        private readonly ITokenService _tokenService;
        private readonly UserManager<User> _userManager;
        private readonly IPasswordEncryptService _passwordEncrypt;

        public LoginService(ILogger<LoginService> logger, ITokenService tokenService, 
            UserManager<User> userManager, IPasswordEncryptService passwordEncrypt)
        {
            _logger = logger;
            _tokenService = tokenService;
            _userManager = userManager;
            _passwordEncrypt = passwordEncrypt;
        }

        public async Task<LoginResponse> LoginByApplication(LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email)
                ?? throw new RequestException("Email or password invalid");

            var validatePass = _passwordEncrypt.Verify(request.Password, user.PasswordHash!);

            if (!validatePass)
                throw new RequestException("Email or password invalid");

            user.RefreshTokenExpiresAt = _tokenService.GenerateExpiration();

            _logger.LogInformation($"new refresh token generated at: {DateTime.UtcNow}, " +
                $"expires at: {user.RefreshTokenExpiresAt}");

            user.RefreshToken = _tokenService.GenerateRefreshToken();

            await _userManager.UpdateAsync(user);

            var roles = await _userManager.GetRolesAsync(user);

            var claims = roles.Select(role => new Claim(ClaimTypes.Role, role)).ToList();

            var accessToken = _tokenService.GenerateToken(claims, user.Id);

            return new LoginResponse() { 
                AccessToken = accessToken, 
                ExpiresAt = (DateTime)user.RefreshTokenExpiresAt, 
                RefreshToken = user.RefreshToken
            };
        }
    }

    public class UserService : IUserService
    {
        private readonly ITokenService _tokenService;
        private readonly UserManager<User> _userManager;
        private readonly IPasswordEncryptService _passwordEncrypt;
        private readonly IUnitOfWork _uow;

        public UserService(ITokenService tokenService, UserManager<User> userManager, 
            IPasswordEncryptService passwordEncrypt, IUnitOfWork uow)
        {
            _tokenService = tokenService;
            _userManager = userManager;
            _passwordEncrypt = passwordEncrypt;
            _uow = uow;
        }

        public async Task<UserResponse> Create(CreateUserRequest request)
        {
            var userByEmail = await _uow.UserRepository.FindByEmail(request.Email);

            if (userByEmail is not null)
                throw new DomainException("This e-mail already is registered");

            using var transaction = await _uow.BeginTransaction();

            try
            {
                var user = new User()
                {
                    Email = request.Email,
                    NormalizedEmail = request.Email.ToUpper(),
                    PasswordHash = _passwordEncrypt.GenerateHash(request.Password),
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    UserName = $"{request.FirstName} {request.LastName}"
                };

                await _uow.UserRepository.Add(user);
                await _uow.Commit();

                await _userManager.AddToRoleAsync(user, "patient");
                await _uow.Commit();

                await transaction.CommitAsync();

                return new UserResponse()
                {
                    CreatedAt = user.CreatedAt,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email
                };
            }
            catch(Exception ex)
            {
                await transaction.RollbackAsync();

                throw;
            }
        }
    }
}
