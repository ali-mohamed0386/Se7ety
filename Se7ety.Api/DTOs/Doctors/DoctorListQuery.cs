using System.ComponentModel.DataAnnotations;
using Se7ety.Api.DTOs.Common;

namespace Se7ety.Api.DTOs.Doctors;

public sealed class DoctorListQuery : PaginationQuery
{
    [MaxLength(100)]
    public string? Specialty { get; set; }
}
