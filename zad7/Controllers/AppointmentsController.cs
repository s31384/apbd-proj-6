using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using zad7.Model;

namespace zad7.Controllers

{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentsController : ControllerBase
    {
        private string connectionString;
        public AppointmentsController(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        [HttpGet]
        public async Task<IActionResult> getAll()
        {
            List<AppointmentDetailsDto> list = new List<AppointmentDetailsDto>();
            using var connection = new SqlConnection(connectionString); 
            await connection.OpenAsync();     
            await using SqlCommand command = new SqlCommand("SELECT * FROM Appointments", connection);
            await using SqlDataReader reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new AppointmentDetailsDto()
                {
                    IdPatient =  (int)reader["IdPatient"],
                });
            }
            return Ok(list);
        }

       /* [HttpGet("{id}")]
        public IActionResult getById([FromRoute] int id)
        {
            
        }*/
        
    }
}
