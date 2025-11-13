using MedSchedule.Application.Requests;
using MedSchedule.Application.Responses;
using MedSchedule.Application.Services;
using MedSchedule.Domain.Enums;
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

        [HttpPost("{id}/check-in")]
        public async Task<IActionResult> CheckInAppointment([FromRoute]Guid id)
        {
            var result = await _appointmentService.CheckInAppointment(id);

            return Ok(result);
        }

        [Authorize(Policy = "OnlyStaffs")]
        [HttpGet("filter")]
        public async Task<IActionResult> FilterAppointments([FromQuery]DateTime? queueDay, [FromQuery]string? specialtyName, 
            [FromQuery]Guid? staffId, [FromQuery]EAppointmentStatus? status, 
            [FromQuery]EPriorityLevel? priorityLevel, [FromQuery]Guid? patientId, [FromQuery]int page, [FromQuery]int perPage)
        {
            var result = await _appointmentService.FilterAppointments(page, perPage, queueDay, specialtyName, staffId, status, priorityLevel, patientId);

            return Ok(result);
        }

        /// <summary>
        /// Create a new appointment for patient and calculate it queue position
        /// </summary>
        /// <param name="request"></param>
        /// <returns>return a response containing data about the appointment created</returns>
        [EnableRateLimiting("create-appointment")]
        [Authorize(Policy = "OnlyPatients")]
        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(typeof(AppointmentResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateNewAppointment([FromBody]AppointmentRequest request)
        {
            var result = await _appointmentService.CreateAppointment(request);

            return Created(string.Empty, result); 
        }
        
        [HttpGet("next")]
        [Authorize(Policy = "OnlyStaffs")]
        public async Task<IActionResult>  NextAppointment()
        {
            var result = await _appointmentService.NextAppointment();

            return Ok(result);
        }

        [HttpPost("{appointmentId}/priority/override")]
        [Authorize(Policy = "OnlyStaffs")]
        public async Task<IActionResult> OverridePriority([FromBody]OverridePriorityRequest request, [FromRoute]Guid appointmentId)
        {
            var result = await _appointmentService.OverridePriority(request, appointmentId);

            return Ok(result);
        }
    }
}
