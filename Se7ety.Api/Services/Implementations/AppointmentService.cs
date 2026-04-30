using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Se7ety.Api.Domain.Entities;
using Se7ety.Api.Domain.Enums;
using Se7ety.Api.DTOs.Appointments;
using Se7ety.Api.Exceptions;
using Se7ety.Api.Helpers;
using Se7ety.Api.Repositories.Interfaces;
using Se7ety.Api.Services.Interfaces;

namespace Se7ety.Api.Services.Implementations;

public sealed class AppointmentService(
    IRepository<Appointment> appointments,
    IRepository<PatientProfile> patientProfiles,
    IRepository<DoctorProfile> doctorProfiles,
    ICurrentUserService currentUser,
    IMapper mapper,
    ILogger<AppointmentService> logger) : IAppointmentService
{
    public async Task<AppointmentResponse> BookAsync(BookAppointmentRequest request, CancellationToken cancellationToken = default)
    {
        var patient = await GetCurrentPatientProfileAsync(cancellationToken);
        var doctor = await doctorProfiles.Query()
            .FirstOrDefaultAsync(profile => profile.Id == request.DoctorProfileId, cancellationToken)
            ?? throw new ApiException(StatusCodes.Status404NotFound, "Doctor was not found.");

        var scheduledAtUtc = SlotSerializer.Normalize(request.ScheduledAtUtc);
        if (scheduledAtUtc <= DateTime.UtcNow)
        {
            throw new ApiException(StatusCodes.Status400BadRequest, "Appointment time must be in the future.");
        }

        var doctorSlots = SlotSerializer.GetSlots(doctor);
        if (doctorSlots.Count == 0 || !doctorSlots.Contains(scheduledAtUtc))
        {
            throw new ApiException(StatusCodes.Status400BadRequest, "Selected time slot is not available for this doctor.");
        }

        var alreadyBooked = await appointments.Query()
            .AnyAsync(appointment =>
                appointment.DoctorProfileId == doctor.Id &&
                appointment.ScheduledAtUtc == scheduledAtUtc &&
                (appointment.Status == AppointmentStatus.Pending || appointment.Status == AppointmentStatus.Accepted),
                cancellationToken);

        if (alreadyBooked)
        {
            throw new ApiException(StatusCodes.Status409Conflict, "Selected time slot is already booked.");
        }

        var appointment = new Appointment
        {
            PatientProfileId = patient.Id,
            DoctorProfileId = doctor.Id,
            ScheduledAtUtc = scheduledAtUtc,
            Notes = string.IsNullOrWhiteSpace(request.Notes) ? null : request.Notes.Trim()
        };

        await appointments.AddAsync(appointment, cancellationToken);
        await appointments.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Patient {PatientProfileId} booked doctor {DoctorProfileId} at {ScheduledAtUtc}.",
            patient.Id,
            doctor.Id,
            scheduledAtUtc);

        return await GetAppointmentResponseAsync(appointment.Id, cancellationToken);
    }

    public async Task<IReadOnlyList<AppointmentResponse>> GetMyPatientBookingsAsync(CancellationToken cancellationToken = default)
    {
        var patient = await GetCurrentPatientProfileAsync(cancellationToken);

        var bookings = await GetAppointmentQuery()
            .Where(appointment => appointment.PatientProfileId == patient.Id)
            .OrderByDescending(appointment => appointment.ScheduledAtUtc)
            .ToListAsync(cancellationToken);

        return bookings.Select(mapper.Map<AppointmentResponse>).ToList();
    }

    public async Task<AppointmentResponse> CancelAsync(Guid appointmentId, CancellationToken cancellationToken = default)
    {
        var patient = await GetCurrentPatientProfileAsync(cancellationToken);
        var appointment = await appointments.Query()
            .FirstOrDefaultAsync(current => current.Id == appointmentId && current.PatientProfileId == patient.Id, cancellationToken)
            ?? throw new ApiException(StatusCodes.Status404NotFound, "Appointment was not found.");

        if (appointment.Status is AppointmentStatus.Cancelled or AppointmentStatus.Rejected)
        {
            throw new ApiException(StatusCodes.Status400BadRequest, "Appointment cannot be cancelled.");
        }

        appointment.Status = AppointmentStatus.Cancelled;
        appointment.UpdatedAtUtc = DateTime.UtcNow;
        appointments.Update(appointment);
        await appointments.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Patient {PatientProfileId} cancelled appointment {AppointmentId}.", patient.Id, appointment.Id);

        return await GetAppointmentResponseAsync(appointment.Id, cancellationToken);
    }

    public async Task<IReadOnlyList<AppointmentResponse>> GetMyDoctorBookingsAsync(CancellationToken cancellationToken = default)
    {
        var doctor = await GetCurrentDoctorProfileAsync(cancellationToken);

        var bookings = await GetAppointmentQuery()
            .Where(appointment => appointment.DoctorProfileId == doctor.Id)
            .OrderByDescending(appointment => appointment.ScheduledAtUtc)
            .ToListAsync(cancellationToken);

        return bookings.Select(mapper.Map<AppointmentResponse>).ToList();
    }

    public async Task<AppointmentResponse> AcceptAsync(Guid appointmentId, CancellationToken cancellationToken = default)
    {
        var appointment = await GetDoctorOwnedAppointmentAsync(appointmentId, cancellationToken);
        if (appointment.Status != AppointmentStatus.Pending)
        {
            throw new ApiException(StatusCodes.Status400BadRequest, "Only pending appointments can be accepted.");
        }

        appointment.Status = AppointmentStatus.Accepted;
        appointment.UpdatedAtUtc = DateTime.UtcNow;
        appointments.Update(appointment);
        await appointments.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Doctor accepted appointment {AppointmentId}.", appointment.Id);

        return await GetAppointmentResponseAsync(appointment.Id, cancellationToken);
    }

    public async Task<AppointmentResponse> RejectAsync(Guid appointmentId, CancellationToken cancellationToken = default)
    {
        var appointment = await GetDoctorOwnedAppointmentAsync(appointmentId, cancellationToken);
        if (appointment.Status != AppointmentStatus.Pending)
        {
            throw new ApiException(StatusCodes.Status400BadRequest, "Only pending appointments can be rejected.");
        }

        appointment.Status = AppointmentStatus.Rejected;
        appointment.UpdatedAtUtc = DateTime.UtcNow;
        appointments.Update(appointment);
        await appointments.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Doctor rejected appointment {AppointmentId}.", appointment.Id);

        return await GetAppointmentResponseAsync(appointment.Id, cancellationToken);
    }

    private async Task<AppointmentResponse> GetAppointmentResponseAsync(Guid appointmentId, CancellationToken cancellationToken)
    {
        var appointment = await GetAppointmentQuery()
            .FirstAsync(current => current.Id == appointmentId, cancellationToken);

        return mapper.Map<AppointmentResponse>(appointment);
    }

    private IQueryable<Appointment> GetAppointmentQuery()
    {
        return appointments.Query()
            .Include(appointment => appointment.DoctorProfile)
            .Include(appointment => appointment.PatientProfile)
            .ThenInclude(profile => profile.User);
    }

    private async Task<PatientProfile> GetCurrentPatientProfileAsync(CancellationToken cancellationToken)
    {
        return await patientProfiles.Query()
            .FirstOrDefaultAsync(profile => profile.UserId == currentUser.UserId, cancellationToken)
            ?? throw new ApiException(StatusCodes.Status404NotFound, "Patient profile was not found.");
    }

    private async Task<DoctorProfile> GetCurrentDoctorProfileAsync(CancellationToken cancellationToken)
    {
        return await doctorProfiles.Query()
            .FirstOrDefaultAsync(profile => profile.UserId == currentUser.UserId, cancellationToken)
            ?? throw new ApiException(StatusCodes.Status404NotFound, "Doctor profile was not found.");
    }

    private async Task<Appointment> GetDoctorOwnedAppointmentAsync(Guid appointmentId, CancellationToken cancellationToken)
    {
        var doctor = await GetCurrentDoctorProfileAsync(cancellationToken);

        return await appointments.Query()
            .FirstOrDefaultAsync(appointment => appointment.Id == appointmentId && appointment.DoctorProfileId == doctor.Id, cancellationToken)
            ?? throw new ApiException(StatusCodes.Status404NotFound, "Appointment was not found.");
    }
}
