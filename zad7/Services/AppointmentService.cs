using Microsoft.Data.SqlClient;
using zad7.Model;

namespace zad7.Services;

public class AppointmentService : IAppointmentService
{
    private string connectionString;
    public AppointmentService(IConfiguration configuration)
    {
        this.connectionString = configuration.GetConnectionString("DefaultConnection");
    }
    public async Task<List<AppointmentsListDto>> getAll(string? status, string? patientLastName)
    {
        List<AppointmentsListDto> list = new List<AppointmentsListDto>();
        using var connection = new SqlConnection(connectionString); 
        await connection.OpenAsync();
        await using SqlCommand command = new SqlCommand("SELECT\n    a.IdAppointment,\n    a.AppointmentDate,\na.InternalNotes,\n    a.Status,\n    a.Reason,\n    p.FirstName + N' ' + p.LastName AS PatientFullName,\n    p.Email AS PatientEmail\nFROM Appointments a\nJOIN Patients p ON p.IdPatient = a.IdPatient\nWHERE (@Status IS NULL OR a.Status = @Status)\n  AND (@PatientLastName IS NULL OR p.LastName = @PatientLastName)\nORDER BY a.AppointmentDate;", connection);
        command.Parameters.AddWithValue("@Status", (object?)status ??  DBNull.Value); 
        command.Parameters.AddWithValue("@PatientLastName", (object?)patientLastName ??  DBNull.Value);
        await using SqlDataReader reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            list.Add(new AppointmentsListDto()
            {
                IdAppointment =  (int)reader["IdAppointment"],
                AppointmentDate = (DateTime)reader["AppointmentDate"],
                Status = (string)reader["Status"],
                Reason = (string)reader["Reason"],
                PatientFullName = (string)reader["PatientFullName"],
                PatientEmail = (string)reader["PatientEmail"]
                    
            });
        }
        return list;
    }
    
    public async Task<AppointmentDetailsDto> getById( int id)
    {
        using var connection = new SqlConnection(connectionString); 
        await connection.OpenAsync();
        await using SqlCommand command = new SqlCommand("SELECT\n    a.IdAppointment,\n    a.AppointmentDate,\n    a.Status,\n    a.Reason,\n    a.InternalNotes,\n    a.CreatedAt,\n    p.FirstName + N' ' + p.LastName AS PatientFullName,\n    p.Email AS PatientEmail,\n    d.FirstName + N' ' + d.LastName AS DoctorFullName,\n    d.LicenseNumber\nFROM Appointments a\nJOIN Patients p ON p.IdPatient = a.IdPatient\nJOIN Doctors d on a.IdDoctor = d.IdDoctor\nWHERE (@IdAppointment IS NULL OR a.IdAppointment = @IdAppointment )",connection);
        command.Parameters.AddWithValue("@IdAppointment", id);
        await using SqlDataReader reader = await command.ExecuteReaderAsync();
        if (!reader.HasRows)
        {
            throw new NotFoundExeption("Appointment not found");
        }
        await reader.ReadAsync();
        AppointmentDetailsDto appointmentDetailsDto = new AppointmentDetailsDto()
        {
            AppointmentDate =  (DateTime)reader["AppointmentDate"],
            DoctorFullName =  (string)reader["DoctorFullName"],
            PatientEmail =   (string)reader["PatientEmail"],
            PatientFullName =  (string)reader["PatientFullName"],
            Status =  (string)reader["Status"],
            Reason =  (string)reader["Reason"],
            InternalNotes =  reader["InternalNotes"] != DBNull.Value ? (string)reader["InternalNotes"] : string.Empty,
            LicenceNumber =   (string)reader["LicenseNumber"],
            CreatedAt = (DateTime)reader["CreatedAt"],
        };
        return appointmentDetailsDto;
    }
    
    
    public async Task<int> add( CreateAppointmentRequestDto createAppointmentRequestDto)
    {
        using var connection = new SqlConnection(connectionString); 
        await connection.OpenAsync();
        await using SqlCommand checkDataCommand = new SqlCommand("SELECT * FROM Doctors WHERE @IdDoctor = IdDoctor;" +
                                                                   "SELECT * FROM Patients WHERE @IdPatient = IdPatient;" +
                                                                   "SELECT * FROM Appointments WHERE @IdDoctor = IdDoctor AND @AppointmentDate = AppointmentDate;", connection);
        checkDataCommand.Parameters.AddWithValue("@IdDoctor", createAppointmentRequestDto.IdDoctor);
        checkDataCommand.Parameters.AddWithValue("@IdPatient", createAppointmentRequestDto.IdDoctor);
        checkDataCommand.Parameters.AddWithValue("@AppointmentDate", createAppointmentRequestDto.AppointmentDate);

        await using SqlDataReader reader = await checkDataCommand.ExecuteReaderAsync();
        if (!reader.HasRows)
        {
            throw new BadRequestExeption("no doctor found");
        }
        await reader.ReadAsync();
        if (!(bool)reader["IsActive"])
        {
            throw new BadRequestExeption("doctor is not active");

        }
            
        await reader.NextResultAsync();
        if (!reader.HasRows)
        {
            throw new BadRequestExeption("no patient found");
        }
        await reader.ReadAsync();
        if (!(bool)reader["IsActive"])
        {
            throw new BadRequestExeption("patient is not active");

        }
        
        await reader.NextResultAsync();
        if (reader.HasRows)
        {
            throw new ConflictExeption("this doctor already have appointment on this time");
        }

        if (createAppointmentRequestDto.AppointmentDate < DateTime.Now)
        {
            throw new BadRequestExeption("wrong time");
        }

        if (createAppointmentRequestDto.Reason.Length > 250)
        {
            throw new BadRequestExeption("reason is too long");
        }
        await reader.CloseAsync();
        SqlCommand insert = new SqlCommand("insert into Appointments(IdPatient,IdDoctor,AppointmentDate,Status,Reason,CreatedAt) values(@IdPatient,@IdDoctor,@AppointmentDate,@Status,@Reason,@CreatedAt);\nSELECT SCOPE_IDENTITY();", connection);
        insert.Parameters.AddWithValue("@IdPatient", createAppointmentRequestDto.IdPatient);
        insert.Parameters.AddWithValue("@IdDoctor", createAppointmentRequestDto.IdDoctor);
        insert.Parameters.AddWithValue("@AppointmentDate",createAppointmentRequestDto.AppointmentDate);
        insert.Parameters.AddWithValue("@Status", "Scheduled");
        insert.Parameters.AddWithValue("@Reason", createAppointmentRequestDto.Reason);
        insert.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
        object? result = await insert.ExecuteScalarAsync();
        return Convert.ToInt32(result);            
    }
    
}