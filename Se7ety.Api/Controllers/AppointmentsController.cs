using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Se7ety.Api.Domain.Enums;
using Se7ety.Api.DTOs.Appointments;
using Se7ety.Api.Services.Interfaces;

namespace Se7ety.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public sealed class AppointmentsController(IAppointmentService appointmentService) : ControllerBase
{
    [HttpPost("patient/book")]
    [Authorize(Roles = nameof(UserRole.Patient))]
    public async Task<ActionResult<AppointmentResponse>> Book(
        BookAppointmentRequest request,
        CancellationToken cancellationToken)
    {
        return Ok(await appointmentService.BookAsync(request, cancellationToken));
    }

    [HttpGet("patient")]
    [Authorize(Roles = nameof(UserRole.Patient))]
    public async Task<ActionResult<IReadOnlyList<AppointmentResponse>>> GetPatientBookings(CancellationToken cancellationToken)
    {
        return Ok(await appointmentService.GetMyPatientBookingsAsync(cancellationToken));
    }

    [HttpPost("patient/{appointmentId:guid}/cancel")]
    [Authorize(Roles = nameof(UserRole.Patient))]
    public async Task<ActionResult<AppointmentResponse>> Cancel(
        Guid appointmentId,
        CancellationToken cancellationToken)
    {
        return Ok(await appointmentService.CancelAsync(appointmentId, cancellationToken));
    }

    [HttpGet("doctor")]
    [Authorize(Roles = nameof(UserRole.Doctor))]
    public async Task<ActionResult<IReadOnlyList<AppointmentResponse>>> GetDoctorBookings(CancellationToken cancellationToken)
    {
        return Ok(await appointmentService.GetMyDoctorBookingsAsync(cancellationToken));
    }

    [HttpPost("doctor/{appointmentId:guid}/accept")]
    [Authorize(Roles = nameof(UserRole.Doctor))]
    public async Task<ActionResult<AppointmentResponse>> Accept(
        Guid appointmentId,
        CancellationToken cancellationToken)
    {
        return Ok(await appointmentService.AcceptAsync(appointmentId, cancellationToken));
    }

    [HttpPost("doctor/{appointmentId:guid}/reject")]
    [Authorize(Roles = nameof(UserRole.Doctor))]
    public async Task<ActionResult<AppointmentResponse>> Reject(
        Guid appointmentId,
        CancellationToken cancellationToken)
    {
        return Ok(await appointmentService.RejectAsync(appointmentId, cancellationToken));
    }
}
