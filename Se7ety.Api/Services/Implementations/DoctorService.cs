using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Se7ety.Api.Domain.Entities;
using Se7ety.Api.Domain.Enums;
using Se7ety.Api.DTOs.Common;
using Se7ety.Api.DTOs.Doctors;
using Se7ety.Api.Exceptions;
using Se7ety.Api.Helpers;
using Se7ety.Api.Repositories.Interfaces;
using Se7ety.Api.Services.Interfaces;

namespace Se7ety.Api.Services.Implementations;

public sealed class DoctorService(
    IRepository<DoctorProfile> doctorProfiles,
    IRepository<Appointment> appointments,
    IMapper mapper) : IDoctorService
{
    public async Task<PagedResponse<DoctorCardResponse>> GetDoctorsAsync(DoctorListQuery query, CancellationToken cancellationToken = default)
    {
        var doctorsQuery = GetCompletedDoctorsQuery();

        if (!string.IsNullOrWhiteSpace(query.Specialty))
        {
            var specialty = query.Specialty.Trim();
            doctorsQuery = doctorsQuery.Where(doctor => doctor.Specialty != null && doctor.Specialty.Contains(specialty));
        }

        return await ToPagedDoctorCardsAsync(doctorsQuery, query.PageNumber, query.PageSize, cancellationToken);
    }

    public async Task<IReadOnlyList<SpecialtyResponse>> GetSpecialtiesAsync(CancellationToken cancellationToken = default)
    {
        return await GetCompletedDoctorsQuery()
            .Where(doctor => doctor.Specialty != null)
            .Select(doctor => doctor.Specialty!)
            .Distinct()
            .OrderBy(specialty => specialty)
            .Select(specialty => new SpecialtyResponse(specialty))
            .ToListAsync(cancellationToken);
    }

    public async Task<PagedResponse<DoctorCardResponse>> SearchDoctorsAsync(DoctorSearchQuery query, CancellationToken cancellationToken = default)
    {
        var doctorsQuery = GetCompletedDoctorsQuery();

        if (!string.IsNullOrWhiteSpace(query.Query))
        {
            var search = query.Query.Trim();
            doctorsQuery = doctorsQuery.Where(doctor =>
                doctor.Name != null && doctor.Name.Contains(search) ||
                doctor.Specialty != null && doctor.Specialty.Contains(search));
        }

        return await ToPagedDoctorCardsAsync(doctorsQuery, query.PageNumber, query.PageSize, cancellationToken);
    }

    public async Task<DoctorDetailsResponse> GetDoctorDetailsAsync(Guid doctorProfileId, CancellationToken cancellationToken = default)
    {
        var doctor = await doctorProfiles.Query()
            .Include(profile => profile.User)
            .Include(profile => profile.Ratings)
            .FirstOrDefaultAsync(profile => profile.Id == doctorProfileId, cancellationToken)
            ?? throw new ApiException(StatusCodes.Status404NotFound, "Doctor was not found.");

        var activeBookedSlots = await appointments.Query()
            .Where(appointment =>
                appointment.DoctorProfileId == doctor.Id &&
                (appointment.Status == AppointmentStatus.Pending || appointment.Status == AppointmentStatus.Accepted))
            .Select(appointment => appointment.ScheduledAtUtc)
            .ToListAsync(cancellationToken);

        var activeBookedLookup = activeBookedSlots
            .Select(SlotSerializer.Normalize)
            .ToHashSet();

        var availableTimes = SlotSerializer.GetSlots(doctor)
            .Where(slot => slot >= SlotSerializer.Normalize(DateTime.UtcNow) && !activeBookedLookup.Contains(slot))
            .OrderBy(slot => slot)
            .ToList();

        return new DoctorDetailsResponse
        {
            Id = doctor.Id,
            Name = doctor.Name,
            Specialty = doctor.Specialty,
            Rating = CalculateRating(doctor),
            Phone = doctor.Phone,
            Location = doctor.Location,
            Email = doctor.User.Email,
            Price = doctor.Price,
            Bio = doctor.Bio,
            AvailableTimes = availableTimes,
            ProfileImageUrl = doctor.ProfileImageUrl
        };
    }

    private IQueryable<DoctorProfile> GetCompletedDoctorsQuery()
    {
        return doctorProfiles.Query()
            .Include(doctor => doctor.Ratings)
            .Where(doctor => doctor.Name != null && doctor.Specialty != null)
            .OrderBy(doctor => doctor.Name);
    }

    private async Task<PagedResponse<DoctorCardResponse>> ToPagedDoctorCardsAsync(
        IQueryable<DoctorProfile> query,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var totalCount = await query.CountAsync(cancellationToken);
        var doctors = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var items = doctors
            .Select(mapper.Map<DoctorCardResponse>)
            .ToList();

        return new PagedResponse<DoctorCardResponse>(
            items,
            pageNumber,
            pageSize,
            totalCount,
            (int)Math.Ceiling(totalCount / (double)pageSize));
    }

    private static double CalculateRating(DoctorProfile doctor)
    {
        return doctor.Ratings.Count == 0
            ? 0
            : Math.Round(doctor.Ratings.Average(rating => rating.Value), 1);
    }
}
