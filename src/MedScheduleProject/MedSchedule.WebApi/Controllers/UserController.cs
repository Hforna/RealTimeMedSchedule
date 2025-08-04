using MedSchedule.Application.Requests;
using MedSchedule.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MedSchedule.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserService _userService;

        public UserController(ILogger<UserController> logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody]CreateUserRequest request)
        {
            var result = await _userService.Create(request);

            return Created(string.Empty, result);
        }
    }
}
