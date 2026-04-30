namespace Se7ety.Api.DTOs.Profiles;

public sealed class DoctorProfileResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? Name { get; set; }
    public string? Specialty { get; set; }
    public string? Location { get; set; }
    public string? Phone { get; set; }
    public decimal Price { get; set; }
    public string? Bio { get; set; }
    public string? ProfileImageUrl { get; set; }
    public double AverageRating { get; set; }
    public IReadOnlyList<DateTime> AvailableSlots { get; set; } = [];
}
