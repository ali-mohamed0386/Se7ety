using System.ComponentModel.DataAnnotations;

namespace Se7ety.Api.DTOs.Profiles;

public sealed class UpdateDoctorSlotsRequest
{
    [Required]
    [MinLength(1)]
    public List<DateTime> AvailableSlots { get; set; } = [];
}
