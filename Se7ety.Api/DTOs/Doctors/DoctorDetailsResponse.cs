namespace Se7ety.Api.DTOs.Doctors;

public sealed class DoctorDetailsResponse
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Specialty { get; set; }
    public double Rating { get; set; }
    public string? Phone { get; set; }
    public string? Location { get; set; }
    public string Email { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string? Bio { get; set; }
    public IReadOnlyList<DateTime> AvailableTimes { get; set; } = [];
    public string? ProfileImageUrl { get; set; }
}
