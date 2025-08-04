using MedSchedule.Application.Requests;
using MedSchedule.Application.Responses;
using MedSchedule.Domain.Aggregates.UserAggregate;
using MedSchedule.Domain.Exceptions;
using MedSchedule.Domain.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedSchedule.Application.Services
{
    public interface IUserService
    {
        public Task<UserResponse> Create(CreateUserRequest request);
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
