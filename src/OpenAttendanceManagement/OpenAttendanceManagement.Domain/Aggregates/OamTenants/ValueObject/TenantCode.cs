using System.ComponentModel.DataAnnotations;
namespace OpenAttendanceManagement.Domain.Aggregates.OamTenants.ValueObject;

public record TenantCode(
    [property: RegularExpression(@"^[a-z0-9]{1,15}$")]
    string Value)
{
    public static TenantCode Default => new(string.Empty);
}