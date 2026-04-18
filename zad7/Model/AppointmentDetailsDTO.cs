namespace zad7.Model;

public class AppointmentDetailsDto
{
    public int IdPatient{ get; set; }
    public   int IdDoctor { get; set; }
    public   DateTime AppointmentDate{ get; set; }
    public  string Status { get; set; }
    public  string Reason { get; set; }
    public   string? InternalNotes { get; set; }
    public   DateTime CreatedOn { get; set; }
}