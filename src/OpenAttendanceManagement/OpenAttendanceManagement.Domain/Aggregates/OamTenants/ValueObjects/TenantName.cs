using System.ComponentModel.DataAnnotations;
namespace OpenAttendanceManagement.Domain.Aggregates.OamTenants.ValueObjects;

public record TenantName(
    [property: RegularExpression(@"^[a-z0-9]{1,30}$")]
    string Value)
{
    public static TenantName Default => new(string.Empty);
}
