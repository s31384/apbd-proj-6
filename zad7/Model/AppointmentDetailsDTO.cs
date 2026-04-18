namespace zad7.Model;

public class AppointmentDetailsDto
{
    public DateTime AppointmentDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public string? InternalNotes { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string PatientFullName { get; set; } = string.Empty;
    public string PatientEmail { get; set; } = string.Empty;
    public string DoctorFullName { get; set; } = string.Empty;
    public string LicenceNumber { get; set; } = string.Empty;
}