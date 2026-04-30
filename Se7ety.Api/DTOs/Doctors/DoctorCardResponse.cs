namespace Se7ety.Api.DTOs.Doctors;

public sealed class DoctorCardResponse
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Specialty { get; set; }
    public double Rating { get; set; }
    public string? ProfileImageUrl { get; set; }
}
