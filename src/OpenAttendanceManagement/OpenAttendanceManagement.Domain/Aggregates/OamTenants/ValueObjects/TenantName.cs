using System.ComponentModel.DataAnnotations;
namespace OpenAttendanceManagement.Domain.Aggregates.OamTenants.ValueObjects;

public record TenantName(
    [property: MaxLength(100)]
    string Value)
{
    public static TenantName Default => new(string.Empty);
}
