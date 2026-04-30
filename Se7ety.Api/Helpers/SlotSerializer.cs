using System.Text.Json;
using Se7ety.Api.Domain.Entities;

namespace Se7ety.Api.Helpers;

public static class SlotSerializer
{
    public static string Serialize(IEnumerable<DateTime> slots)
    {
        var normalizedSlots = slots
            .Select(Normalize)
            .Distinct()
            .OrderBy(slot => slot)
            .ToList();

        return JsonSerializer.Serialize(normalizedSlots);
    }

    public static IReadOnlyList<DateTime> Deserialize(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return [];
        }

        try
        {
            return JsonSerializer.Deserialize<List<DateTime>>(value)?
                .Select(Normalize)
                .Distinct()
                .OrderBy(slot => slot)
                .ToList() ?? [];
        }
        catch (JsonException)
        {
            return [];
        }
    }

    public static IReadOnlyList<DateTime> GetSlots(DoctorProfile doctorProfile)
    {
        return Deserialize(doctorProfile.WorkingTimes);
    }

    public static DateTime Normalize(DateTime value)
    {
        var utc = value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Local => value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
        };

        return new DateTime(utc.Year, utc.Month, utc.Day, utc.Hour, utc.Minute, 0, DateTimeKind.Utc);
    }
}
