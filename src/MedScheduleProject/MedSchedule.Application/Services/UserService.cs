using MedSchedule.Application.Requests;
using MedSchedule.Application.Responses;
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

        public Task<UserResponse> Create(CreateUserRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
