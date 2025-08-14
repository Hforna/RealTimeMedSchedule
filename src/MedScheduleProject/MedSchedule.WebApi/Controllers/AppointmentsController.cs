using MedSchedule.Application.Requests;
using MedSchedule.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace MedSchedule.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentsController : ControllerBase
    {
        private readonly ILogger<AppointmentsController> _logger;
        private readonly IAppointmentService _appointmentService;

        public AppointmentsController(ILogger<AppointmentsController> logger, IAppointmentService appointmentService)
        {
            _logger = logger;
            _appointmentService = appointmentService;
        }

        [EnableRateLimiting("create-appointment")]
        [Authorize(Roles = "patient")]
        [HttpPost]
        public async Task<IActionResult> CreateNewAppointment([FromBody]AppointmentRequest request)
        {
            var result = await _appointmentService.CreateAppointment(request);

            return Created(string.Empty, result); 
        }
    }
}
