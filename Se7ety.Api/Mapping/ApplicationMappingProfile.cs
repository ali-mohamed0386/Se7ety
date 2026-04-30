using AutoMapper;
using Se7ety.Api.Domain.Entities;
using Se7ety.Api.DTOs.Appointments;
using Se7ety.Api.DTOs.Doctors;
using Se7ety.Api.DTOs.Profiles;

namespace Se7ety.Api.Mapping;

public sealed class ApplicationMappingProfile : Profile
{
    public ApplicationMappingProfile()
    {
        CreateMap<PatientProfile, PatientProfileResponse>()
            .ForMember(destination => destination.Email, options => options.MapFrom(source => source.User.Email));

        CreateMap<DoctorProfile, DoctorProfileResponse>()
            .ForMember(destination => destination.Email, options => options.MapFrom(source => source.User.Email))
            .ForMember(destination => destination.AverageRating, options => options.MapFrom(source =>
                source.Ratings.Count == 0 ? 0 : Math.Round(source.Ratings.Average(rating => rating.Value), 1)))
            .ForMember(destination => destination.AvailableSlots, options => options.Ignore());

        CreateMap<DoctorProfile, DoctorCardResponse>()
            .ForMember(destination => destination.Rating, options => options.MapFrom(source =>
                source.Ratings.Count == 0 ? 0 : Math.Round(source.Ratings.Average(rating => rating.Value), 1)));

        CreateMap<Appointment, AppointmentResponse>()
            .ForMember(destination => destination.Status, options => options.MapFrom(source => source.Status.ToString()))
            .ForMember(destination => destination.DoctorId, options => options.MapFrom(source => source.DoctorProfileId))
            .ForMember(destination => destination.DoctorName, options => options.MapFrom(source => source.DoctorProfile.Name))
            .ForMember(destination => destination.DoctorSpecialty, options => options.MapFrom(source => source.DoctorProfile.Specialty))
            .ForMember(destination => destination.DoctorProfileImageUrl, options => options.MapFrom(source => source.DoctorProfile.ProfileImageUrl))
            .ForMember(destination => destination.PatientId, options => options.MapFrom(source => source.PatientProfileId))
            .ForMember(destination => destination.PatientEmail, options => options.MapFrom(source => source.PatientProfile.User.Email))
            .ForMember(destination => destination.PatientPhoneNumber, options => options.MapFrom(source => source.PatientProfile.PhoneNumber));
    }
}
