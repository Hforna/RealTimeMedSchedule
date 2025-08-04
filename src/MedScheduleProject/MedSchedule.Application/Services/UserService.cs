using MedSchedule.Application.Requests;
using MedSchedule.Application.Responses;
using MedSchedule.Domain.Aggregates.UserAggregate;
using MedSchedule.Domain.Exceptions;
using MedSchedule.Domain.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
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

            return new LoginResponse() { 
                AccessToken = _tokenService.GenerateToken(claims, user.Id), 
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

        public UserService(ITokenService tokenService, UserManager<User> userManager, IPasswordEncryptService passwordEncrypt)
        {
            _tokenService = tokenService;
            _userManager = userManager;
            _passwordEncrypt = passwordEncrypt;
        }

        public async Task<UserResponse> Create(CreateUserRequest request)
        {
            var userByEmail = await _userManager.FindByEmailAsync(request.Email);

            if (userByEmail is not null)
                throw new DomainException("This e-mail already is registered");

            var user = new User()
            {
                Email = request.Email,
                PasswordHash = _passwordEncrypt.GenerateHash(request.Password),
                FirstName = request.FirstName,
                LastName = request.LastName
            };

            await _userManager.CreateAsync(user);
            await _userManager.AddToRoleAsync(user, "patient");

            return new UserResponse() { 
                CreatedAt = user.CreatedAt, 
                FirstName = request.FirstName, 
                LastName = request.LastName, 
                Email = request.Email };
        }
    }
}
