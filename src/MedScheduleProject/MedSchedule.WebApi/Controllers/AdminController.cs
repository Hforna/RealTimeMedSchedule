using MedSchedule.Application.Requests;
using MedSchedule.Application.Services;
using MedSchedule.WebApi.Filter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;

namespace MedSchedule.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AuthenticatedUser]
    [Authorize(Roles = "admin")]
    public class AdminController : ControllerBase
    {
        private readonly ILogger<AdminController> _logger;
        private readonly IAdminService _adminService;

        public AdminController(ILogger<AdminController> logger, IAdminService adminService)
        {
            _logger = logger;
            _adminService = adminService;
        }

        [HttpPut("staffs/specialty")]
        public async Task<IActionResult> AssignSpecialtyToStaff([FromBody]SetSpecialtyToStaffRequest request)
        {
            var result = await _adminService.AssignSpecialtyToStaff(request);

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllStaffs([FromBody]StaffsPaginatedRequest request)
        {
            var result = await _adminService.GetAllStaffsPaginated(request);

            return Ok(result);
        }

        [HttpPost("staffs")]
        public async Task<IActionResult> CreateNewStaff([FromBody]CreateNewStaffRequest request)
        {
            var result = await _adminService.CreateNewStaff(request);

            return Created(string.Empty, result);
        }
    }
}
