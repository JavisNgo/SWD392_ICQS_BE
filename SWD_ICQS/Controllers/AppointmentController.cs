using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SWD_ICQS.ModelsView;
using SWD_ICQS.Services.Implements;
using SWD_ICQS.Services.Interfaces;

namespace SWD_ICQS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController : Controller
    {
        private readonly IAppointmentService _appointmentService;

        public AppointmentController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        [AllowAnonymous]
        [HttpGet("/Appointments")]
        public async Task<IActionResult> GetAllAppointments()
        {
            try
            {
                var appointments = _appointmentService.GetAllAppointment();
                return Ok(appointments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while retrieving appointments. Error message: {ex.Message}");
            }
        }

        [AllowAnonymous]
        [HttpGet("/api/v1/appointments/contractor/{contractorId}")]
        public ActionResult<IEnumerable<AppointmentView>> GetAppointmentByContractorId(int contractorId)
        {
            try
            {
                var appointments = _appointmentService.GetAppointmentByContractorId(contractorId);
                return Ok(appointments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [AllowAnonymous]
        [HttpGet("/api/v1/appointments/customer/{customerId}")]
        public ActionResult<IEnumerable<AppointmentView>> GetAppointmentByCustomerId(int customerId)
        {
            try
            {
                var appointments = _appointmentService.GetAppointmentByCustomerId(customerId);
                return Ok(appointments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
