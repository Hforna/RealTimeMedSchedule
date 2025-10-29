using MedSchedule.Application.Requests;
using MedSchedule.Application.Responses;
using MedSchedule.Application.Services;
using MedSchedule.WebApi.Filter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace MedSchedule.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AuthenticatedUser]
    public class AppointmentsController : ControllerBase
    {
        private readonly ILogger<AppointmentsController> _logger;
        private readonly IAppointmentService _appointmentService;

        public AppointmentsController(ILogger<AppointmentsController> logger, IAppointmentService appointmentService)
        {
            _logger = logger;
            _appointmentService = appointmentService;
        }

        /// <summary>
        /// Create a new appointment for patient and calculate it queue position
        /// </summary>
        /// <param name="request"></param>
        /// <returns>return a response containing data about the appointment created</returns>
        [EnableRateLimiting("create-appointment")]
        //[Authorize(Policy = "OnlyPatients")]
        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(typeof(AppointmentResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateNewAppointment([FromBody]AppointmentRequest request)
        {
            var result = await _appointmentService.CreateAppointment(request);

            return Created(string.Empty, result); 
        }
        
        [HttpGet("next")]
        //[Authorize(Policy = "OnlyStaffs")]
        public async Task<IActionResult>  NextAppointment()
        {
            var result = await _appointmentService.NextAppointment();

            return Ok(result);
        }
    }
}
