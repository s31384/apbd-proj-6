using Microsoft.AspNetCore.Mvc;
using zad7.Model;

namespace zad7.Services;

public interface IAppointmentService
{
    public Task<AppointmentDetailsDto> getById(int id);
    public Task<List<AppointmentsListDto>> getAll(string? status, string? patientLastName);
    public Task<int> add(CreateAppointmentRequestDto createAppointmentRequestDto);


}