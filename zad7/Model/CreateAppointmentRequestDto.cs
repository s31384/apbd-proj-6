namespace zad7.Model;

public class CreateAppointmentRequestDto
{
    public int IdPatient { get; set; }
    public int IdDoctor { get; set; }
    public string Reason{ get; set; } = string.Empty;
    public DateTime AppointmentDate { get; set; }
}