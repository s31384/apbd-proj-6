using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using zad7.Model;
using zad7.Services;

namespace zad7.Controllers

{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentsController : ControllerBase
    {
        private readonly IAppointmentService _service;

        public AppointmentsController(IConfiguration configuration, IAppointmentService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> getAll(
            [FromQuery] string? status,
            [FromQuery] string? patientLastName)
        {
            List<AppointmentsListDto> list = await _service.getAll(status, patientLastName);
            return Ok(list);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> getById([FromRoute] int id)
        {
            AppointmentDetailsDto appointmentDetailsDto;
            try
            {
                appointmentDetailsDto = await _service.getById(id);
            }
            catch (NotFoundExeption e)
            {
                return NotFound(e.Message);
            }

            return Ok(appointmentDetailsDto);
        }

        [HttpPost]
        public async Task<IActionResult> add([FromBody] CreateAppointmentRequestDto createAppointmentRequestDto)
        {
            int id;
            try
            {
                id = await _service.add(createAppointmentRequestDto);
            }
            catch (BadRequestExeption e)
            {
                return BadRequest(e.Message);
            }
            catch (ConflictExeption e)
            {
                return Conflict(e.Message);
            }
            Console.Write(id);
            return CreatedAtAction(nameof(getById), new{id}, createAppointmentRequestDto);

        }
    }
}
